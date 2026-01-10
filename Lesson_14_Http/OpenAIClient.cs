using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lesson_14_Http;

public class OpenAIClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OpenAIClient(string apiKey, string model = "gpt-5-mini")
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _model = model;
    }

    public async Task<string> GetCompletionAsync(ChatHistory history)
    {
        var payload = new
        {
            model = _model,
            messages = history.GetMessagesForOpenAI()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(json);
        var completion = result.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return completion ?? string.Empty;
    }

    public async Task<string> GetResponseAsync(ChatHistory history)
    {
        var payload = new
        {
            model = _model,
            input = history.GetMessagesForOpenAI()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/responses", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(json);
        var completion = result.RootElement
            .GetProperty("output")[0]
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        return completion ?? string.Empty;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

