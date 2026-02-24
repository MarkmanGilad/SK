using OpenAI.Chat;

public class OpenAI_SDK
{
    private readonly ChatClient GPTModel;
    
    public OpenAI_SDK(string model)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ChatClient(model, OpenAIKey);
    }

    public async Task<string> Call(string userMessage)
    {
        // Send the chat completion request
        var completion = await GPTModel.CompleteChatAsync(userMessage);

        // Get the response content
        return completion.Value.Content[0].Text;
    }
}
