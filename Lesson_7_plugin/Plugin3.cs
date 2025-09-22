// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Globalization;

namespace Lesson_7_plugin
{
    public class Plugin3
    {
        public class DateTimePlugin
        {
            [KernelFunction("get_date")]
            [Description("Get today's date")]
            [return: Description("Return date format DD-MM-YYYY.")]
            public string Today()
            {
                return DateTime.Now.ToString("dd-MM-yyyy");
            }
            
            [KernelFunction("get_time")]
            [Description("Get Current time")]
            public string Current_time()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }
        
        public async Task Run()
        {
            var OpenAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var model = "gpt-4.1-mini";
            // Create a Semantic Kernel builder instance
            var builder = Kernel.CreateBuilder();

            // Add the OpenAI chat completion service to the kernel builder
            builder.AddOpenAIChatCompletion(model, OpenAIKey);

            // Add the the plugin from class
            builder.Plugins.AddFromType<DateTimePlugin>("DateTime");

            // Build the kernel with the configured services
            var kernel = builder.Build();

            // Retrieve the chat completion service from the kernel
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // Enable auto planning of the agent
            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            ChatHistory history = new ChatHistory();
            history.AddSystemMessage("You are a helpful assistant that always answer to the point");


            while (true)
            {
                Console.Write(">> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }

                history.AddUserMessage(userMessage);

                string string_builder = "";

                var stream = chatService.GetStreamingChatMessageContentsAsync(history, settings, kernel);

                await foreach (var chunk in stream)
                {
                    Console.Write(chunk);
                    string_builder += chunk.Content;
                }

                Console.WriteLine();
                history.AddAssistantMessage(string_builder);
            }
        }
    }
}
