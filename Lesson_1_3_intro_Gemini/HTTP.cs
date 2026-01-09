using DotNetEnv;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Lesson_1_3_intro_Gemini
{
    public class HTTP
    {
        public static async Task Main(string[] args)
        {
            string GeminiAPIKey = "AIzaSyBPsnKZ8jJZHzxuhR3ZuDQ1rEF8ztQeWv8";
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

            Console.WriteLine(completion);
        }
    }
}
