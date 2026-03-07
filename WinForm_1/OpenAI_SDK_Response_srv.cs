#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_SDK_Response_srv
{
    private readonly ResponsesClient GPTModel;
    private string ? previousResponseId = null;
    private readonly string model;

    public OpenAI_SDK_Response_srv(string model)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(OpenAIKey);
        this.model = model;
    }

    public async Task<string> Call(string userMessage)
    {
        ResponseResult response = await GPTModel.CreateResponseAsync(model,
            userMessage, previousResponseId);

        previousResponseId = response.Id;

        return response.GetOutputText();
    }

    public void ClearHistory()
    {
        previousResponseId = null;
    }
}

