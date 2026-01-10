using DotNetEnv;
using System.Text;
using System.Text.Json;


namespace Lesson_1_2_http
{
    public class Gemini_Http
    {
        public static async Task<string> Call()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            string model = "gemini-2.5-flash";

            // User prompt message
            Console.Write("You (Http) >> ");
            string userMessage = Console.ReadLine();

            // Create HTTP client
            using var httpClient = new HttpClient();

            // Build the API URL
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={GeminiAPIKey}";

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
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Parse the response
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);

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
