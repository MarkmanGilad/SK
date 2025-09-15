using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_7_plugin
{
    public class Plugin4
    {
        public class DateTimePlugin
        {
            [KernelFunction, Description("Return today's date in a friendly format")]
            public string GetDate()
            {
                return DateTime.Now.ToString("dddd, MMMM dd, yyyy");
            }

            [KernelFunction, Description("Return the current time (HH:mm:ss)")]
            public string GetTime()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        static async Task Main()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4.1-mini",
                apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var kernel = builder.Build();

            kernel.ImportPluginFromObject(new DateTimePlugin(), "DateTime");

            // Use the newer FunctionChoiceBehavior.Auto()
            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var history = new ChatHistory();
            history.AddSystemMessage("You can use tools GetDate and GetTime as needed.");

            Console.Write("Ask your question: ");
            string userQuestion = Console.ReadLine();
            history.AddUserMessage(userQuestion);

            var reply = await kernel.GetRequiredService<IChatCompletionService>()
                                   .GetChatMessageContentAsync(history, settings, kernel);

            Console.WriteLine(reply.Content);
        }
    }
}
