
#pragma warning disable OPENAI001
using OpenAI.Responses;
using System.Text.Json;

public class Tools_GPT_Thinking
{
    private string? ContainerId { get; set; }

    public async Task Run(bool thinking = true)
    {
        var systemPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use TavilySearch when you need to search the web.
                Use GetSchema to understand the SQL database structure.
                Use RetrieveTable to run SELECT queries on the SQL database.
                Use ExecuteNonQuery only when the user explicitly asks to change data in the SQL database.
                You may use the Code Interpreter tool when needed.
                Use it for calculations, data analysis, plots and code-based reasoning.
                Always EXECUTE code using the Code Interpreter tool. Never just show code as text.
                """;

        var tools = new DateTimeTools();
        var tavily = new TavilySearch();
        var sqlTools = new SQLTools();

        var toolsList = CreateTools();

        var openai = new OpenAI_Tools(
            model: "gpt-5.4",
            systemPrompt: systemPrompt,
            tools: toolsList);

        while (true)
        {
            Console.Write("Ask your question: ");
            var message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) return;

            const int maxSteps = 5;

            ContainerId = null;

            if (thinking)
            {
                Console.WriteLine();
                Console.WriteLine("====================== Thinking ======================");
                Console.WriteLine();
            }

            var response = await openai.Call(message);

            for (int step = 0; step < maxSteps; step++)
            {
                int count = 0;
                var toolOutputs = new List<ResponseItem>();

                foreach (var item in response.OutputItems)
                {
                    if (item is CodeInterpreterCallResponseItem)
                    {
                        count++;
                    }
                    if (item is FunctionCallResponseItem call)
                    {
                        count++;
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

                if (thinking)
                {
                    PrintContent(response.OutputItems);
                    PrintContent(toolOutputs);
                }

                SaveFiles(response);

                if (count == 0)
                {
                    break;
                }

                if (step == maxSteps - 1)
                {
                    toolOutputs.Add(ResponseItem.CreateUserMessageItem(
                        "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));

                    response = await openai.Call(toolOutputs);
                    SaveFiles(response);
                    break;
                }

                response = await openai.Call(toolOutputs);
            }

            Console.WriteLine();
            Console.WriteLine("====================== Final Answer ======================");
            Console.WriteLine();

            PrintContent(response.OutputItems);

            Console.WriteLine("========================== END ===========================");
            Console.WriteLine();
        }
    }

    private void PrintContent(IEnumerable<ResponseItem> items)
    {
        foreach (var item in items)
        {
            if (item is FunctionCallResponseItem fc)
            {
                Console.WriteLine($"--- Tool call: {fc.FunctionName} ---");
                Console.WriteLine($"Arguments: {fc.FunctionArguments}");
            }
            else if (item is FunctionCallOutputResponseItem fo)
            {
                Console.WriteLine($"--- Tool result ---");
                Console.WriteLine(fo.FunctionOutput);
                Console.WriteLine();
            }
            else if (item is CodeInterpreterCallResponseItem ci)
            {
                if (!string.IsNullOrWhiteSpace(ci.ContainerId))
                {
                    ContainerId = ci.ContainerId;
                }

                Console.WriteLine($"--- Executable code (python) ---");
                Console.WriteLine(ci.Code);

                foreach (var output in ci.Outputs)
                {
                    if (output is CodeInterpreterCallLogsOutput logs)
                    {
                        Console.WriteLine($"--- Code execution output ---");
                        Console.WriteLine(logs.Logs);
                        Console.WriteLine();
                    }
                    else if (output is CodeInterpreterCallImageOutput)
                    {
                        Console.WriteLine($"--- Code execution image ---");
                        Console.WriteLine();
                    }
                }
            }
            else if (item is MessageResponseItem msg)
            {
                foreach (var part in msg.Content)
                {
                    if (!string.IsNullOrWhiteSpace(part.Text))
                    {
                        Console.WriteLine(part.Text);
                    }
                }
            }
        }
    }

    private void SaveFiles(ResponseResult response)
    {
        foreach (var item in response.OutputItems)
        {
            if (item is CodeInterpreterCallResponseItem ci)
            {
                if (!string.IsNullOrWhiteSpace(ci.ContainerId))
                {
                    ContainerId = ci.ContainerId;
                }

                foreach (var output in ci.Outputs)
                {
                    if (output is CodeInterpreterCallImageOutput image)
                    {
                        SaveImage(image.ImageUri, ci.ContainerId);
                    }
                }
            }
            else if (item is MessageResponseItem msg)
            {
                foreach (var part in msg.Content)
                {
                    foreach (var annotation in part.OutputTextAnnotations)
                    {
                        if (annotation is not ContainerFileCitationMessageAnnotation file)
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(file.ContainerId))
                        {
                            ContainerId = file.ContainerId;
                        }

                        SaveContainerFile(file.ContainerId, file.FileId, file.Filename);
                    }

                    var imageUriProperty = part.GetType().GetProperty("ImageUri");
                    if (imageUriProperty?.GetValue(part) is Uri imageUri)
                    {
                        SaveImage(imageUri, ContainerId);
                    }
                }
            }
        }
    }

    private static List<ResponseTool> CreateTools()
    {
        var noParamsSchema = BinaryData.FromString("""
            {
                "type":"object",
                "properties":{},
                "required":[],
                "additionalProperties":false
            }
            """);

        var tavilySchema = BinaryData.FromString("""
            {
                "type":"object",
                "properties":{"query":{"type":"string"}},
                "required":["query"],
                "additionalProperties":false
            }
            """);

        var sqlSchema = BinaryData.FromString("""
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
            functionDescription: "Get today's date");

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

        var codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(
            new CodeInterpreterToolContainer(
                CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration(
                    Array.Empty<string>())));

        return new List<ResponseTool> {getDateTool, getTimeTool, tavilyTool, getSchemaTool, retrieveTableTool, executeNonQueryTool, codeInterpreterTool};
    }

    private static void SaveImage(Uri imageUri, string? containerId)
    {
        try
        {
            var path = imageUri.AbsolutePath;
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) ext = ".png";

            using var http = new HttpClient();
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }
            if (!string.IsNullOrEmpty(containerId))
            {
                http.DefaultRequestHeaders.Add("OpenAI-Container", containerId);
            }

            var bytes = http.GetByteArrayAsync(imageUri).GetAwaiter().GetResult();

            var fileName = $"plot_{DateTime.Now:yyyyMMdd_HHmmss_fff}{ext}";
            var folder = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Plots"));
            Directory.CreateDirectory(folder);
            var fullPath = Path.Combine(folder, fileName);
            System.IO.File.WriteAllBytes(fullPath, bytes);
            Console.WriteLine($"[Image saved: {fullPath}]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Image save failed: {ex.Message}]");
        }
    }

    private static void SaveContainerFile(string? containerId, string? fileId, string? filename)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(containerId) || string.IsNullOrWhiteSpace(fileId))
            {
                Console.WriteLine("[File save failed: missing container id or file id]");
                return;
            }

            using var http = new HttpClient();
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }

            http.DefaultRequestHeaders.Add("OpenAI-Container", containerId);

            var uri = new Uri($"https://api.openai.com/v1/containers/{containerId}/files/{fileId}/content");
            var bytes = http.GetByteArrayAsync(uri).GetAwaiter().GetResult();

            var safeName = string.IsNullOrWhiteSpace(filename)
                ? $"file_{DateTime.Now:yyyyMMdd_HHmmss_fff}.bin"
                : Path.GetFileName(filename);

            var folder = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Plots"));
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{safeName}");
            System.IO.File.WriteAllBytes(fullPath, bytes);
            Console.WriteLine($"[File saved: {fullPath}]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[File save failed: {ex.Message}]");
        }
    }
}
