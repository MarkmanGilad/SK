using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;


namespace Lesson_7_plugin
{
    public class Plugin1
    {
        public async Task Run()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4.1-mini",
                apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var kernel = builder.Build();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            string system = """
                Always reply with JSON only:
                {
                  "Thought": "why you chose the action",
                  "Action": "GetDate" or "FinalAnswer",
                  "Input": "" // empty for GetDate, or the final answer text for FinalAnswer
                }
                
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
            return DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }
    }
}
