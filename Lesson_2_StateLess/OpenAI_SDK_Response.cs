#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel;

    public OpenAI_SDK_Response(string model)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(model, OpenAIKey);
    }
    public async Task<string> Call(string userMessage)
    {
        // Send the chat completion request
        var response = await GPTModel.CreateResponseAsync(userMessage);

        // Get the response content
        return response.Value.GetOutputText();
    }
}

