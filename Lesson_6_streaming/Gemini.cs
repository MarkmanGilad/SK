using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using DotNetEnv;

namespace Lesson_6_streaming
{
    internal class Gemini
    {
        public static async Task Main(string[] args)
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");

            string model = "gemini-2.5-flash";

            // Create a Semantic Kernel builder instance
            var builder = Kernel.CreateBuilder();

            // Add the Gemini chat completion service to the kernel builder
            builder.AddGoogleAIGeminiChatCompletion(model, GeminiAPIKey);

            // Build the kernel with the configured services
            var kernel = builder.Build();

            // Retrieve the chat completion service from the kernel
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            ChatHistory history = new ChatHistory();
            history.AddSystemMessage("You are a helpful assistant");

            while (true)
            {
                Console.Write("You (Gemini)>> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }
                history.AddUserMessage(userMessage);

                string string_builder = "";

                var stream = chatService.GetStreamingChatMessageContentsAsync(
                    chatHistory: history);

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
