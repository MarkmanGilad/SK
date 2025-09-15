using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lesson_7_plugin
{
    public class Plugin5
    {
        private class Step
        {
            public string Action { get; set; }
            public int arg1 { get; set; }
            public int arg2 { get; set; }
            public string Answer { get; set; }
        }

        private static class MathTool
        {
            public static int Add(int a, int b) => a + b;
        }

        public async Task Run()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion("gpt-4.1-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var kernel = builder.Build();
            var chat = kernel.GetRequiredService<IChatCompletionService>();

            string system = """
                Reply in JSON only:
                {
                  "Action": "AddNumbers" | "FinalAnswer",
                  "arg1": number,
                  "arg2": number,
                  "Answer": "" // used only when Action = "FinalAnswer"
                }
                If asked to add numbers, set Action="AddNumbers" and provide arg1 and arg2.
                Otherwise respond with Action="FinalAnswer" and put your full answer in Answer.
                """;

            var history = new ChatHistory(system);

            Console.Write("Ask your question: ");
            var user = Console.ReadLine();
            history.AddUserMessage(user);

            var reply = await chat.GetChatMessageContentAsync(history);
            var step = JsonSerializer.Deserialize<Step>(reply.Content);

            if (step.Action == "AddNumbers")
            {
                var sum = MathTool.Add(step.arg1, step.arg2);
                history.AddAssistantMessage(reply.Content);
                history.AddAssistantMessage($"[TOOL] AddNumbers => {sum}");
                history.AddUserMessage("Return FinalAnswer with your explanation in Answer.");

                var finalReply = await chat.GetChatMessageContentAsync(history);
                var finalStep = JsonSerializer.Deserialize<Step>(finalReply.Content);
                Console.WriteLine(finalStep.Answer);
            }
            else
            {
                Console.WriteLine(step.Answer);
            }
        }

        
    }
}

