#pragma warning disable OPENAI001
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.VectorStores;

public class OpenAI_Tools
{
    private readonly ResponsesClient GPTModel;
    private readonly OpenAIFileClient FileModel;
    private readonly VectorStoreClient VectorStoreModel;
    private readonly List<ResponseItem> history = new();
    private readonly string model;
    private readonly string? systemPrompt;
    private readonly List<ResponseTool>? tools;
    private readonly string? schema;

    public OpenAI_Tools(
        string model,
        string? systemPrompt = null,
        List<ResponseTool>? tools = null,
        string? schema = null)
    {
        var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        GPTModel = new ResponsesClient(OpenAIKey);
        FileModel = new OpenAIFileClient(OpenAIKey);
        VectorStoreModel = new VectorStoreClient(OpenAIKey);
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

    public async Task<string> UploadFile(string filePath)
    {
        var uploadedFile = await FileModel.UploadFileAsync(filePath, FileUploadPurpose.UserData);
        return uploadedFile.Value.Id;
    }

    public async Task<string> CreateVectorStore(string fileId)
    {
        var vectorStore = await VectorStoreModel.CreateVectorStoreAsync(
            new VectorStoreCreationOptions
            {
                FileIds = { fileId }
            });

        return vectorStore.Value.Id;
    }

    public async Task DeleteVectorStore(string vectorStoreId)
    {
        await VectorStoreModel.DeleteVectorStoreAsync(vectorStoreId);
    }

    public async Task DeleteUploadedFile(string fileId)
    {
        await FileModel.DeleteFileAsync(fileId);
    }

    public async Task<string> AddFileToVectorStore(string vectorStoreId, string filePath)
    {
        var fileId = await UploadFile(filePath);
        await VectorStoreModel.AddFileToVectorStoreAsync(vectorStoreId, fileId);
        return fileId;
    }
}
