using OpenAI.Chat;

public class OpenAI_SDK
{
    private readonly ChatClient GPTModel;
    private readonly List<ChatMessage> history = new();
    private readonly ChatCompletionOptions config;

    public OpenAI_SDK(string model, string? systemPrompt = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ChatClient(model, OpenAIKey);

        config = new ChatCompletionOptions
        {
            //MaxOutputTokenCount = , // Max tokens in the response
        };

        if (!string.IsNullOrEmpty(systemPrompt))
        {
            history.Add(new SystemChatMessage(systemPrompt));
        }
    }

    public async Task<string> Call(string userMessage)
    {
        history.Add(new UserChatMessage(userMessage));
               
        var completion = await GPTModel.CompleteChatAsync(history, config);
        var text = completion.Value.Content[0].Text;

        history.Add(new AssistantChatMessage(text));
        return text;
    }
}
