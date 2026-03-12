#pragma warning disable OPENAI001
using OpenAI.Responses;
using System.Text.Json;

public class Tools_GPT
{
    public async Task Run()
    {
        var systemPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use TavilySearch when you need to search the web.
                Use GetSchema to understand the SQL database structure.
                Use RetrieveTable to run SELECT queries on the SQL database.
                Use ExecuteNonQuery only when the user explicitly asks to change data in the SQL database.
                """;

        var tools = new DateTimeTools();
        var tavily = new TavilySearch();
        var sqlTools = new SQLTools();

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

        var sqlSchema = BinaryData.FromString(
            """
            {
                "type":"object",
                "properties":{"sql":{"type":"string"}},
                "required":["sql"],
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

        var getSchemaTool = ResponseTool.CreateFunctionTool(
            functionName: "GetSchema",
            functionParameters: noParamsSchema,
            strictModeEnabled: true,
            functionDescription: "Get the structure of the SQL database");

        var retrieveTableTool = ResponseTool.CreateFunctionTool(
            functionName: "RetrieveTable",
            functionParameters: sqlSchema,
            strictModeEnabled: true,
            functionDescription: "Run a SELECT query on the SQL database and return the result as JSON");

        var executeNonQueryTool = ResponseTool.CreateFunctionTool(
            functionName: "ExecuteNonQuery",
            functionParameters: sqlSchema,
            strictModeEnabled: true,
            functionDescription: "Run INSERT, UPDATE, or DELETE on the SQL database " +
            "and return the number of affected rows");

        var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool>
            {
                getDateTool,
                getTimeTool,
                tavilyTool,
                getSchemaTool,
                retrieveTableTool,
                executeNonQueryTool
            });

        while (true)
        {
            Console.Write("Ask your question: ");
            var message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) return;

            const int maxSteps = 5;

            var response = await openai.Call(message);
            bool finalResponsePrinted = false;

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

                        try
                        {
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
                                var argsStr = call.FunctionArguments.ToString();
                                var args = JsonSerializer.Deserialize<Dictionary<string, string>>(argsStr);
                                var query = args["query"];
                                toolResult = await tavily.Search(query);
                            }
                            else if (call.FunctionName == "GetSchema")
                            {
                                toolResult = sqlTools.GetSchema();
                            }
                            else if (call.FunctionName == "RetrieveTable")
                            {
                                var argsStr = call.FunctionArguments.ToString();
                                var args = JsonSerializer.Deserialize<Dictionary<string, string>>(argsStr);
                                var sql = args["sql"];
                                toolResult = sqlTools.RetrieveTable(sql);
                            }
                            else if (call.FunctionName == "ExecuteNonQuery")
                            {
                                var argsStr = call.FunctionArguments.ToString();
                                var args = JsonSerializer.Deserialize<Dictionary<string, string>>(argsStr);
                                var sql = args["sql"];
                                toolResult = sqlTools.ExecuteNonQuery(sql).ToString();
                            }
                            else
                            {
                                toolResult = "Unknown tool: " + call.FunctionName;
                            }
                        }
                        catch (Exception ex)
                        {
                            toolResult = "Tool error: " + ex.Message;
                        }

                        toolOutputs.Add(ResponseItem.CreateFunctionCallOutputItem(call.CallId, toolResult));
                    }
                }

                if (count == 0)
                {
                    Console.WriteLine(response.GetOutputText());
                    finalResponsePrinted = true;
                    break;
                }

                if (step == maxSteps - 1)
                {
                    toolOutputs.Add(ResponseItem.CreateUserMessageItem(
                        "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));

                    response = await openai.Call(toolOutputs);
                    Console.WriteLine(response.GetOutputText());
                    finalResponsePrinted = true;
                    break;
                }

                response = await openai.Call(toolOutputs);
            }

            if (!finalResponsePrinted)
            {
                Console.WriteLine("Max iterations reached.");
            }
        }
    }
}
