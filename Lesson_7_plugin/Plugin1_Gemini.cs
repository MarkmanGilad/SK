using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Globalization;
using System.Text.Json;

using DotNetEnv;

namespace Lesson_7_plugin
{
    public class Plugin1_Gemini
    {
        public class AgentStep
        {
            public string Thought { get; set; }
            public string Action { get; set; }   // "GetDate" or "FinalAnswer"
            public string Input { get; set; }
        }

        public static class DateTool
        {
            public static string GetDate()
            {
                return DateTime.Now.ToString("dddd, MMMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
            }
        }

        public async Task Run()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");

            var builder = Kernel.CreateBuilder();
            builder.AddGoogleAIGeminiChatCompletion(
                modelId: "gemini-2.5-flash",
                apiKey: GeminiAPIKey);
            var kernel = builder.Build();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            string system = """
                Always reply with JSON only:
                {
                  "Thought": "why you chose the action",
                  "Action": "GetDate" or "FinalAnswer",
                  "Input": "" // empty for GetDate, or the final answer text for FinalAnswer
                }
                Don't add "json" or other text before the json itself.
                If you need today's date/day to answer, set Action="GetDate" and leave Input empty.
                If you can answer without it, set Action="FinalAnswer" and put the full answer in Input.
                """;

            var history = new ChatHistory();
            history.AddSystemMessage(system);

            Console.Write("Ask your question: ");
            string userQuestion = Console.ReadLine();
            history.AddUserMessage(userQuestion);

            // First model reply
            var reply = await chatService.GetChatMessageContentAsync(history);
            var stepObj = JsonSerializer.Deserialize<AgentStep>(reply.Content);

            if (stepObj.Action == "GetDate")
            {
                string result = DateTool.GetDate();

                // Give result back to model
                history.AddAssistantMessage(reply.Content);
                history.AddAssistantMessage("[TOOL] GetDate => " + result);
                history.AddUserMessage("Now return FinalAnswer with the result.");

                // Ask model again for final answer
                var finalReply = await chatService.GetChatMessageContentAsync(history);
                var finalStep = JsonSerializer.Deserialize<AgentStep>(finalReply.Content);

                Console.WriteLine(finalStep.Input);
            }
            else if (stepObj.Action == "FinalAnswer")
            {
                Console.WriteLine(stepObj.Input);
            }
        }
    }
}
