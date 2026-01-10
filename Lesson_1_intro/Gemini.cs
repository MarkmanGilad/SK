using DotNetEnv;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lesson_1_intro
{
    public class Gemini
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

            // User prompt message
            Console.Write(">> ");
            string userMessage = Console.ReadLine();
            
            // Send the user's message to the chat model and await the response
            var result = await chatService.GetChatMessageContentAsync(userMessage);

            Console.WriteLine(result);
            
        }
    }
}
