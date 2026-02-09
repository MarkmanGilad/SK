using DotNetEnv;
using OpenAI.Chat;

namespace Lesson_1_2_http
{
    public class OpenAI_SDK
    {
        public static async Task<string> Call()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string model = "gpt-5-mini";

            // User prompt message
            Console.Write("You (program) >> ");
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