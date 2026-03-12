#pragma warning disable OPENAI001

using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel;
    private readonly string model;

    public OpenAI_SDK_Response(string model)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        this.model = model;
        GPTModel = new ResponsesClient(OpenAIKey);
    }
    public async Task<string> Call(string userMessage)
    {
        // Send the chat completion request
        var response = await GPTModel.CreateResponseAsync(model, userMessage);

        // Get the response content
        return response.Value.GetOutputText();
    }
}

