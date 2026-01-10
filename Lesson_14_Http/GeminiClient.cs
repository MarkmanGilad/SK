using System.Text;
using System.Text.Json;

namespace Lesson_14_Http;

public class GeminiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _apiKey;

    public GeminiClient(string apiKey, string model = "gemini-2.5-flash")
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> GetCompletionAsync(ChatHistory history)
    {
        // Build the API URL with the API key as a query parameter
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        // Get system instruction if available
        var systemInstruction = history.GetSystemMessageForGemini();

        // Build the payload dynamically
        object payload;
        if (systemInstruction != null)
        {
            payload = new
            {
                contents = history.GetMessagesForGemini(),
                systemInstruction = new
                {
                    parts = new[]
                    {
                        new { text = systemInstruction }
                    }
                }
            };
        }
        else
        {
            payload = new
            {
                contents = history.GetMessagesForGemini()
            };
        }

        // Send the HTTP request
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
        var response = await _httpClient.PostAsync(url, content);
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

        return completion ?? string.Empty;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
