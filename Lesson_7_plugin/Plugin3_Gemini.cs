using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI; // Use OpenAI Connector namespace
using System.ComponentModel;
using DotNetEnv;

namespace Lesson_7_plugin
{
    public class Plugin3_Gemini
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
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            // Use the OpenAI-compatible model name if needed
            var model = "gemini-2.5-flash";

            // Create a Semantic Kernel builder instance
            var builder = Kernel.CreateBuilder();

            
            // OPENAI CONNECTOR POINTING TO GOOGLE ENDPOINT + No Streaming !!!!!
            builder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: GeminiAPIKey,
                endpoint: new Uri("https://generativelanguage.googleapis.com/v1beta/openai/") 
            );

            // Add the the plugin from class
            builder.Plugins.AddFromType<DateTimePlugin>("DateTime");

            // Build the kernel with the configured services
            var kernel = builder.Build();

            // Retrieve the chat completion service from the kernel
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // Enable auto planning using OpenAI Settings
            // Gemini's OpenAI endpoint can be sensitive to the newer FunctionChoiceBehavior serialization.
            // Converting to the more compatible ToolCallBehavior.AutoInvokeKernelFunctions often fixes 400 Bad Request errors.
            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            ChatHistory history = new ChatHistory();
            
            // Simplified system message usually works better with OpenAI protocol
            history.AddSystemMessage("You are a helpful assistant.");

            while (true)
            {
                Console.Write("Ask Gemini (via OpenAI Proto)>> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }

                history.AddUserMessage(userMessage);

                // Streaming function calling with third-party OpenAI endpoints can be fragile.
                // Switching back to non-streaming for the most reliable tool execution.
                var result = await chatService.GetChatMessageContentAsync(history, settings, kernel);

                Console.WriteLine(result.Content);

                Console.WriteLine();
                history.AddAssistantMessage(result.Content);
            }
        }
    }
}
