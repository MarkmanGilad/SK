using DotNetEnv;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lesson_1_intro
{
    public class Gemini_SK
    {
        public static async Task<String> Call()
        {
            Env.TraversePath().Load();
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

            // User prompt message
            Console.Write("You (Gemini_SK)>> ");
            string userMessage = Console.ReadLine();
            
            // Send the user's message to the chat model and await the response
            var result = await chatService.GetChatMessageContentAsync(userMessage);

            return result.Content;
            
        }
    }
}
