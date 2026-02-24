#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel;
    private string previousResponseId = "";

    public OpenAI_SDK_Response(string model)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(model, OpenAIKey);
    }
    public async Task<string> Call(string userMessage)
    {
        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem(userMessage),
        ];

        CreateResponseOptions options = new(inputItems)
        {
            PreviousResponseId = previousResponseId
        };
        
        ResponseResult response = await GPTModel.CreateResponseAsync(options);

        previousResponseId = response.Id;

        return response.GetOutputText();
    }
}

