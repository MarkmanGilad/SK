using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;


namespace Lesson_7_plugin
{
    public class Plugin2
    {
        public class AgentStep
        {
            public string Thought { get; set; }
            public string Action { get; set; }   // "GetDate", "GetTime", or "FinalAnswer"
            public string Input { get; set; }
        }

        public static class Tools
        {
            public static string GetDate()
            {
                return DateTime.Now.ToString("dddd, MMMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
            }

            public static string GetTime()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        public async Task Run()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4.1-mini",
                apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var kernel = builder.Build();

            var chat = kernel.GetRequiredService<IChatCompletionService>();

            string system = """
                Always reply with JSON only:
                {
                  "Thought": "why you chose the action",
                  "Action": "GetDate" or "GetTime" or "FinalAnswer",
                  "Input": "" //for tool calls, or the final answer text for FinalAnswer
                }
                If you need today's date, use GetDate.
                If you need the current time, use GetTime.
                After a tool result is provided, return FinalAnswer with the result.
                """;

            var history = new ChatHistory();
            history.AddSystemMessage(system);

            Console.Write("Ask your question: ");
            string userQuestion = Console.ReadLine();
            history.AddUserMessage(userQuestion);

            // Loop: let model call tools until FinalAnswer
            for (int step = 0; step < 5; step++)
            {
                var reply = await chat.GetChatMessageContentAsync(history);
                var stepObj = JsonSerializer.Deserialize<AgentStep>(reply.Content);

                if (stepObj.Action == "GetDate")
                {
                    string result = Tools.GetDate();
                    history.AddAssistantMessage(reply.Content);
                    history.AddAssistantMessage("[TOOL] GetDate => " + result);
                    history.AddUserMessage("""
                        Use the tool result above. 
                        If you still need information, choose the a tool; 
                        Otherwise return the full answer in Input."
                        """);
                    continue;
                }

                if (stepObj.Action == "GetTime")
                {
                    string result = Tools.GetTime();
                    history.AddAssistantMessage(reply.Content);
                    history.AddAssistantMessage("[TOOL] GetTime => " + result);
                    history.AddUserMessage("""
                        Use the tool result above. 
                        If you still need information, choose the a tool; 
                        Otherwise return the full answer in Input."
                        """);
                    continue;
                }

                if (stepObj.Action == "FinalAnswer")
                {
                    Console.WriteLine(stepObj.Input);
                    return;
                }
            }
        }

    }
    
}
