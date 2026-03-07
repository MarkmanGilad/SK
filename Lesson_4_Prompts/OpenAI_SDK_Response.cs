#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel; 
    private readonly List<ResponseItem> history = new();
    private readonly string model;

    public OpenAI_SDK_Response(string model, string? systemPrompt = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(OpenAIKey);
        this.model = model;

        if (!string.IsNullOrEmpty(systemPrompt))
        {
            history.Add(ResponseItem.CreateSystemMessageItem(systemPrompt));
        }
    }

    public async Task<string> Call(string userMessage)
    {
        history.Add(ResponseItem.CreateUserMessageItem(userMessage));
        
        ResponseResult response = await GPTModel.CreateResponseAsync(model, history);

        history.Add(ResponseItem.CreateAssistantMessageItem(response.GetOutputText()));

        return response.GetOutputText();
    }
}

