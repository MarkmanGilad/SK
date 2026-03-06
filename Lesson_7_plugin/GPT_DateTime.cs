#pragma warning disable OPENAI001
using OpenAI.Responses;

public class GPT_DateTime
{
    private readonly ResponsesClient client;
    private readonly string model;
    private readonly string? systemPrompt;
    private readonly List<ResponseTool> tools;
    private readonly DateTimeTools toolsImpl = new();
    private readonly List<ResponseItem> history = new();

    public GPT_DateTime(string model)
    {
        var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
        this.client = new ResponsesClient(model, openAIKey);
        this.model = model;
        
        this.systemPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                """;

        var noParamsSchema = BinaryData.FromString("""{ "type":"object", "properties":{}, "required":[], "additionalProperties":false }""");

        var getDateTool = ResponseTool.CreateFunctionTool(
            functionName: "GetDate",
            functionParameters: noParamsSchema,
            strictModeEnabled: true,
            functionDescription: "Get today's date");

        var getTimeTool = ResponseTool.CreateFunctionTool(
            functionName: "GetTime",
            functionParameters: noParamsSchema,
            strictModeEnabled: true,
            functionDescription: "Get the current time");

        this.tools = new List<ResponseTool> { getDateTool, getTimeTool };
    }

    public async Task<string> Call(string userPrompt, int maxSteps = 5)
    {
        history.Add(ResponseItem.CreateUserMessageItem(userPrompt));

        ResponseResult response = await CreateResponse();

        for (int step = 0; step < maxSteps; step++)
        {
            bool hasToolCalls = false;
            foreach (var item in response.OutputItems)
            {
                if (item is FunctionCallResponseItem )
                {
                    var call = (FunctionCallResponseItem)item;
                    hasToolCalls = true;
                    string toolResult;

                    if (call.FunctionName == "GetDate")
                    {
                        toolResult = toolsImpl.GetDate();
                    }
                    else if (call.FunctionName == "GetTime")
                    {
                        toolResult = toolsImpl.GetTime();
                    }
                    else
                    {
                        toolResult = "Unknown tool: " + call.FunctionName;
                    }

                    history.Add(ResponseItem.CreateFunctionCallOutputItem(call.CallId, toolResult));
                }
            }

            if (!hasToolCalls)
            {
                return response.GetOutputText();
            }

            if (step == maxSteps - 1)
            {
                history.Add(ResponseItem.CreateUserMessageItem(
                    "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));
            }

            response = await CreateResponse();
        }

        return "Max iterations reached.";
    }

    private async Task<ResponseResult> CreateResponse()
    {
        var config = CreateConfig();

        ResponseResult response = await client.CreateResponseAsync(config);

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

        foreach (var tool in tools)
        {
            config.Tools.Add(tool);
        }

        return config;
    }
}
