using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lesson_13_WinForms;

public class OpenAIClient
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
            messages = history.GetMessagesForApi()
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
            input = history.GetMessagesForApi()
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

public class ChatHistory
{
    private readonly List<Message> _messages = new();

    public void AddSystemMessage(string content)
    {
        _messages.Add(new Message { Role = "system", Content = content });
    }

    public void AddUserMessage(string content)
    {
        _messages.Add(new Message { Role = "user", Content = content });
    }

    public void AddAssistantMessage(string content)
    {
        _messages.Add(new Message { Role = "assistant", Content = content });
    }

    public void Clear()
    {
        _messages.Clear();
    }

    public IReadOnlyList<Message> GetMessages()
    {
        return _messages.AsReadOnly();
    }

    public object GetMessagesForApi()
    {
        var apiMessages = new List<object>();
        foreach (var m in _messages)
        {
            apiMessages.Add(new { role = m.Role, content = m.Content });
        }
        return apiMessages;
    }

    public class Message
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}

