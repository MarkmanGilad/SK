using DotNetEnv;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lesson_1_intro
{
    public class OpenAI_SK
    {
        public static async Task<string> Call()
        {
            Env.TraversePath().Load();
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string model = "gpt-5-mini";

            // Create a Semantic Kernel builder instance
            var builder = Kernel.CreateBuilder();

            // Add the OpenAI chat completion service to the kernel builder
            builder.AddOpenAIChatCompletion(model, OpenAIKey);

            // Build the kernel with the configured services
            var kernel = builder.Build();

            // Retrieve the chat completion service from the kernel
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // User prompt message
            Console.Write("You (OpenAI_SK)>> ");
            string userMessage = Console.ReadLine();

            // Send the user's message to the chat model and await the response
            var result = await chatService.GetChatMessageContentAsync(userMessage);

            return result.Content;
        }
    }
}