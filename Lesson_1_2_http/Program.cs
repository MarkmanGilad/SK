using DotNetEnv;
using System.Text;
using System.Text.Json;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
string model = "gpt-5-mini";

// User prompt message
Console.Write("You (program) >> ");
var userMessage = Console.ReadLine();

// Create HTTP client
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

// Build the API URL
var url = "https://api.openai.com/v1/chat/completions";

// Create the request payload
var payload = new
{
    model = model,
    messages = new[]
    {
        new
        {
            role = "user",
            content = userMessage
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
    .GetProperty("choices")[0]
    .GetProperty("message")
    .GetProperty("content")
    .GetString();

Console.WriteLine($"OpenAI >> {completion}");
