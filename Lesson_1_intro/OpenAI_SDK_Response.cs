#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

namespace Lesson_1_intro
{
    public class OpenAI_SDK_Response
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
            var client = new ResponsesClient(OpenAIKey);

            // Send the chat completion request
            var response = await client.CreateResponseAsync(model, userMessage);

            // Get the response content
            return response.Value.GetOutputText();
        }
    }
}