#pragma warning disable OPENAI001
using OpenAI.Responses;

public class OpenAI_Tools
{
    private readonly ResponsesClient GPTModel;
    private readonly List<ResponseItem> history = new();
    private readonly string model;
    private readonly string? systemPrompt;
    private readonly List<ResponseTool>? tools;
    private readonly string? schema;

    public OpenAI_Tools(string model, string? systemPrompt = null, List<ResponseTool>? tools = null, string? schema = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(OpenAIKey);
        this.model = model;
        this.systemPrompt = systemPrompt;
        this.tools = tools;
        this.schema = schema;
    }

    public Task<ResponseResult> Call(string userMessage)
    {
        var newItems = new List<ResponseItem> { ResponseItem.CreateUserMessageItem(userMessage) };
        return Call(newItems);
    }

    public async Task<ResponseResult> Call(List<ResponseItem> newItems)
    {
        foreach (var item in newItems)
        {
            history.Add(item);
        }

        var config = CreateConfig();

        ResponseResult response = await GPTModel.CreateResponseAsync(config);

        foreach (var item in response.OutputItems)
        {
            history.Add(item);
        }

        return response;
    }

    private CreateResponseOptions CreateConfig()
    {
        var config = new CreateResponseOptions
        {
            Model = model,
            Instructions = systemPrompt
        };

        foreach (var item in history)
        {
            config.InputItems.Add(item);
        }

        if (tools is not null)
        {
            foreach (var tool in tools)
            {
                config.Tools.Add(tool);
            }
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

        return config;
    }

    public void ClearHistory()
    {
        history.Clear();
    }
}
