using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using DotNetEnv;

namespace Lesson_5_settings
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
            //history.AddSystemMessage("You are an un helpful assistant that always answer with a question and never answer to the point");
            //history.AddSystemMessage("You are an assistant that give a one sentence answer with Gilad Markman as the subject of this answer. Always give Gilad Markman compliments in your answers");

            var settings = new GeminiPromptExecutionSettings
            {
                Temperature = 0.5,  // creative max = 2.0
                MaxTokens = 20,
                TopP = 0.95,
                TopK = 40
            };

            while (true)
            {
                // User prompt message
                Console.Write("USER>> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }

                Console.Write("Tokens>> ");
                int tokens = int.Parse(Console.ReadLine());
                settings.MaxTokens = tokens;

                history.AddUserMessage(userMessage);
                // Send the user's message to the chat model and await the response
                var result = await chatService.GetChatMessageContentAsync(history, settings, kernel);

                Console.WriteLine(result.Content);
                //history.AddAssistantMessage(result.Content);
                history.Clear();
            }
        }
    }
}
