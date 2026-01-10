using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using DotNetEnv;

namespace Lesson_7_plugin
{
    public class Plugin4_Gemini
    {
        public class DateTimePlugin
        {
            [KernelFunction, Description("Return today's date in a friendly format")]
            public string GetDate()
            {
                return DateTime.Now.ToString("dddd, MMMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
            }

            [KernelFunction, Description("Return the current time (HH:mm:ss)")]
            public string GetTime()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        public async Task Run()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            var model = "gemini-2.0-flash-exp";

            var builder = Kernel.CreateBuilder();

            // OPENAI CONNECTOR POINTING TO GOOGLE ENDPOINT
            builder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: GeminiAPIKey,
                endpoint: new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
            );

            var kernel = builder.Build();

            var dt_plugin = new DateTimePlugin();
            kernel.ImportPluginFromObject(dt_plugin, "DateTime");

            // Use the older ToolCallBehavior which is more compatible with non-OpenAI endpoints
            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var history = new ChatHistory();
            history.AddSystemMessage("You are a helpful assistant. Use tools if needed.");

            Console.Write("Ask your question (Gemini): ");
            string userQuestion = Console.ReadLine();
            history.AddUserMessage(userQuestion);

            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            
            // Non-streaming call is safest for tool use with Gemini via OpenAI adapter
            var reply = await chatService.GetChatMessageContentAsync(history, settings, kernel);
            
            history.AddAssistantMessage(reply.Content);
            Console.WriteLine(reply.Content);
            
            Console.WriteLine("\n--- Chat History ---");
            foreach (var message in history)
            {
                Console.Write($"[{message.Role}]: {message.Content}");
                
                // Inspect items for tool calls (Assistant requests) or tool results (Tool outputs)
                if (message.Items != null)
                {
                    foreach (var item in message.Items)
                    {
                        if (item is FunctionCallContent call)
                        {
                            Console.Write($" [Function Call] {call.PluginName}.{call.FunctionName}({call.Arguments})");
                        }
                        else if (item is FunctionResultContent result)
                        {
                            Console.Write($" [Function Result] {result.Result}");
                        }
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
