#pragma warning disable OPENAI001
using OpenAI.Responses;
using System.Text.Json;

public class Tools4_GPT
{
    public async Task Run()
    {
        var systemPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use TavilySearch when you need to search the web.
                """;

        var tools = new DateTimeTools();
        var tavily = new TavilySearch();

        var noParamsSchema = BinaryData.FromString("""
            {
                "type":"object", 
                "properties":{}, 
                "required":[], 
                "additionalProperties":false 
            }
            """);

        var tavilySchema = BinaryData.FromString(
            """
            { 
                "type":"object", 
                "properties":{"query":{"type":"string"}}, 
                "required":["query"], 
                "additionalProperties":false 
            }
            """);

        var getDateTool = ResponseTool.CreateFunctionTool(
            functionName: "GetDate",
            functionParameters: noParamsSchema,
            strictModeEnabled: true,
            functionDescription: "Get today's date"
            );

        var getTimeTool = ResponseTool.CreateFunctionTool(
            functionName: "GetTime",
            functionParameters: noParamsSchema,
            strictModeEnabled: true,
            functionDescription: "Get the current time");

        var tavilyTool = ResponseTool.CreateFunctionTool(
            functionName: "TavilySearch",
            functionParameters: tavilySchema,
            strictModeEnabled: true,
            functionDescription: "Search the web and return results for a query");

        var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool> { getDateTool, getTimeTool, tavilyTool });

        Console.Write("Ask your question: ");
        var message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) return;

        const int maxSteps = 5;

        var response = await openai.Call(message);

        for (int step = 0; step < maxSteps; step++)
        {
            int count = 0;
            var toolOutputs = new List<ResponseItem>();
            foreach (var item in response.OutputItems)
            {
                if (item is FunctionCallResponseItem)
                {
                    count++;
                    var call = (FunctionCallResponseItem)item;
                    string toolResult;

                    if (call.FunctionName == "GetDate")
                    {
                        toolResult = tools.GetDate();
                    }
                    else if (call.FunctionName == "GetTime")
                    {
                        toolResult = tools.GetTime();
                    }
                    else if (call.FunctionName == "TavilySearch")
                    {
                        var args_str = call.FunctionArguments.ToString();
                        var args = JsonSerializer.Deserialize<Dictionary<string, string>>(args_str);
                        var query = args["query"];
                        toolResult = await tavily.Search(query);
                    }
                    else
                    {
                        toolResult = "Unknown tool: " + call.FunctionName;
                    }

                    toolOutputs.Add(ResponseItem.CreateFunctionCallOutputItem(call.CallId, toolResult));
                }
            }

            if (count == 0)
            {
                Console.WriteLine(response.GetOutputText());
                return;
            }

            if (step == maxSteps - 1)
            {
                toolOutputs.Add(ResponseItem.CreateUserMessageItem(
                    "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));

                response = await openai.Call(toolOutputs);
                Console.WriteLine(response.GetOutputText());
                return;
            }

            response = await openai.Call(toolOutputs);
        }

        Console.WriteLine("Max iterations reached.");
    }
}
