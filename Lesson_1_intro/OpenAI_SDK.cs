using DotNetEnv;
using OpenAI.Chat;

namespace Lesson_1_intro
{
    public class OpenAI_SDK
    {
        public static async Task<string> Call()
        {
            Env.TraversePath().Load();
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string model = "gpt-5-mini";

            // User prompt message
            Console.Write("You (OpenAI_SDK) >> ");
            var userMessage = Console.ReadLine();

            // Create OpenAI client
            var client = new ChatClient(model, OpenAIKey);

            // Send the chat completion request
            var completion = await client.CompleteChatAsync(userMessage);

            // Get the response content
            return completion.Value.Content[0].Text;
        }
    }
}