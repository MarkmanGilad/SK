using DotNetEnv;
using Google.GenAI;


namespace Lesson_1_intro
{
    public class Gemini_SDK
    {
        public static async Task<string> Call()
        {
            Env.TraversePath().Load();
            var geminiKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            string model = "gemini-2.5-flash";

            // User prompt message
            Console.Write("You (Gemini_SDK) >> ");
            var userMessage = Console.ReadLine();

            // Create Gemini model
            var geminiModel = new Client(apiKey:geminiKey);
            
            // Send the request
            var response = await geminiModel.Models.GenerateContentAsync(
                model: "gemini-2.5-flash",
                contents: userMessage
            );

            // Get the response content
            var text = response.Candidates[0].Content.Parts[0].Text;
            return text;
        }
    }
}