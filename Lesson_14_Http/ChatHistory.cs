namespace Lesson_14_Http;

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

    public object GetMessagesForOpenAI()
    {
        var apiMessages = new List<object>();
        foreach (var m in _messages)
        {
            apiMessages.Add(new { role = m.Role, content = m.Content });
        }
        return apiMessages;
    }

    public object GetMessagesForGemini()
    {
        var contents = new List<object>();

        foreach (var message in _messages)
        {
            // Gemini uses "model" role instead of "assistant"
            string role;
            if (message.Role == "assistant")
            {
                role = "model";
            }
            else
            {
                role = message.Role;
            }

            // Skip system messages - they will be handled separately in systemInstruction
            if (role == "system")
            {
                continue;
            }

            contents.Add(new
            {
                role = role,
                parts = new[]
                {
                    new { text = message.Content }
                }
            });
        }

        return contents;
    }

    public string? GetSystemMessageForGemini()
    {
        // Get the first system message for Gemini's systemInstruction field
        var systemMessage = _messages.FirstOrDefault(m => m.Role == "system");
        return systemMessage?.Content;
    }

    public class Message
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
