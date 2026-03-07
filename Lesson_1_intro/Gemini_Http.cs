using DotNetEnv;
using System.Text;
using System.Text.Json;

namespace Lesson_1_intro
{
    public class Gemini_Http
    {
        public static async Task<string> Call()
        {
            Env.TraversePath().Load();
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            string model = "gemini-2.5-flash";

            // User prompt message
            Console.Write("You (Gemini_Http) >> ");
            string userMessage = Console.ReadLine();

            // Create HTTP client
            using var httpClient = new HttpClient();

            // Build the API URL
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:" +
                $"generateContent?key={GeminiAPIKey}";

            // Create the request payload
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = userMessage }
                        }
                    }
                }
            };

            // Send the HTTP request
            var content = new StringContent(JsonSerializer.Serialize(payload), 
                Encoding.UTF8, "application/json");

            // First await: sends the request and waits until the response headers (status code) arrive
            // The response body may not be downloaded yet at this point
            var response = await httpClient.PostAsync(url, content);
            
            // Throws HttpRequestException if not success
            response.EnsureSuccessStatusCode();
                        
            // Second await: headers arrived with PostAsync, we wait for the full response body to download
            var json = await response.Content.ReadAsStringAsync();

            // Parse the raw JSON string into a navigable document tree
            var result = JsonDocument.Parse(json);

            // Navigate the JSON tree: candidates[0] -> content -> parts[0] -> text
            var completion = result.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return completion;
        }
    }
}
