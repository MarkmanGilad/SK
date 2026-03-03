#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_SDK_Response
{
    private readonly ResponsesClient GPTModel;
    private readonly List<ResponseItem> history = new();
    private readonly CreateResponseOptions config;

    public OpenAI_SDK_Response(string model, string? systemPrompt = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(model, OpenAIKey);

        config = new CreateResponseOptions
        {
            Model = model, // Model name to run (for example: gpt-5.2 / gpt-5-mini)
        };

        // System Prompt
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            config.Instructions = systemPrompt;
        }
    }


    public async Task<string> Call(string userMessage, string ? schema = null)
    {
        history.Add(ResponseItem.CreateUserMessageItem(userMessage));

        config.InputItems.Clear();
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

        foreach (var item in response.OutputItems)
        {
            history.Add(item);
        }

        return response.GetOutputText();
    }
}

