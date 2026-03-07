#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel;
    private readonly List<ResponseItem> history = new();
    private readonly string model;
    private readonly string? systemPrompt;

    public OpenAI_SDK_Response(string model, string? systemPrompt = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(OpenAIKey);
        this.model = model;
        this.systemPrompt = systemPrompt;
    }

    public async Task<string> Call(string userMessage, string? schema = null)
    {
        history.Add(ResponseItem.CreateUserMessageItem(userMessage));


        var config = new CreateResponseOptions
        {
            Model = model, 
            Instructions = systemPrompt
        };

        foreach (var item in history)
        {
            config.InputItems.Add(item);
        }

        if (schema is not null)
        {
            config.TextOptions = new ResponseTextOptions
            {
                TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                  jsonSchemaFormatName: "response_schema",
                  jsonSchema: BinaryData.FromString(schema),
                  jsonSchemaIsStrict: true)
            };
        }

        ResponseResult response = await GPTModel.CreateResponseAsync(config);

        history.Add(ResponseItem.CreateAssistantMessageItem(response.GetOutputText()));

        return response.GetOutputText();

    }
}

