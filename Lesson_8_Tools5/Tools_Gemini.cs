using Google.GenAI.Types;

public class Tools_Gemini
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
                You may use the Gemini Code Execution tool when needed.
                Use it for calculations, data analysis, and code-based reasoning.
            
            """;

        var tools = new DateTimeTools();
        var tavily = new TavilySearch();
        var sqlTools = new SQLTools();

        var toolsList = CreateTools();

        var gemini = new Gemini_Tools(
            model: "gemini-3.1-pro-preview",
            systemPrompt: systemPrompt,
            tools: toolsList);

        while (true)
        {
            Console.Write("Ask your question: ");
            var message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) return;

            const int maxSteps = 5;

            var response = await gemini.Call(message);
            bool finalResponsePrinted = false;

            for (int step = 0; step < maxSteps; step++)
            {
                var parts = response.Candidates[0].Content.Parts;

                int count = 0;
                var toolOutputMessage = new Content { Role = "user", Parts = new List<Part>() };

                foreach (var part in parts)
                {
                    if (part.FunctionCall is not null)
                    {
                        count++;
                        var call = part.FunctionCall;
                        string toolResult;

                        try
                        {
                            if (call.Name == "GetDate")
                            {
                                toolResult = tools.GetDate();
                            }
                            else if (call.Name == "GetTime")
                            {
                                toolResult = tools.GetTime();
                            }
                            else if (call.Name == "TavilySearch")
                            {
                                var query = call.Args["query"].ToString();
                                toolResult = await tavily.Search(query);
                            }
                            else if (call.Name == "GetSchema")
                            {
                                toolResult = sqlTools.GetSchema();
                            }
                            else if (call.Name == "RetrieveTable")
                            {
                                var sql = call.Args["sql"].ToString();
                                toolResult = sqlTools.RetrieveTable(sql);
                            }
                            else if (call.Name == "ExecuteNonQuery")
                            {
                                var sql = call.Args["sql"].ToString();
                                toolResult = sqlTools.ExecuteNonQuery(sql).ToString();
                            }
                            else
                            {
                                toolResult = "Unknown tool: " + call.Name;
                            }
                        }
                        catch (Exception ex)
                        {
                            toolResult = "Tool error: " + ex.Message;
                        }

                        toolOutputMessage.Parts.Add(new Part
                        {
                            FunctionResponse = new FunctionResponse
                            {
                                Name = call.Name,
                                Response = new Dictionary<string, object> { { "result", toolResult } }
                            }
                        });
                    }
                }

                if (count == 0)
                {
                    HandleFinalResponse(response);
                    finalResponsePrinted = true;
                    break;
                }

                if (step == maxSteps - 1)
                {
                    toolOutputMessage.Parts.Add(new Part
                    {
                        Text = "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."
                    });

                    response = await gemini.Call(new List<Content> { toolOutputMessage });

                    HandleFinalResponse(response);
                    finalResponsePrinted = true;
                    break;
                }

                response = await gemini.Call(new List<Content> { toolOutputMessage });
            }

            if (!finalResponsePrinted)
            {
                Console.WriteLine("Max iterations reached.");
            }
        }
    }

    private static void HandleFinalResponse(GenerateContentResponse response)
    {
        var parts = response.Candidates?[0].Content?.Parts;
        if (parts is null) return;

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part.Text))
            {
                Console.WriteLine(part.Text);
            }
            else if (part.ExecutableCode is not null)
            {
                Console.WriteLine();
                Console.WriteLine($"--- Executable code ({part.ExecutableCode.Language}) ---");
                Console.WriteLine(part.ExecutableCode.Code);
                Console.WriteLine("-------------------------------");
            }
            else if (part.CodeExecutionResult is not null)
            {
                Console.WriteLine();
                Console.WriteLine($"--- Code execution output ({part.CodeExecutionResult.Outcome}) ---");
                Console.WriteLine(part.CodeExecutionResult.Output);
                Console.WriteLine("-----------------------------------");
            }
            else if (part.InlineData is not null
                && part.InlineData.MimeType is string mime
                && mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                && part.InlineData.Data is byte[] bytes
                && bytes.Length > 0)
            {
                SaveImage(bytes, mime);
            }
        }
    }

    private static List<Tool> CreateTools()
    {
        var noParamsSchema = new Schema { Type = Google.GenAI.Types.Type.Object };

        var tavilySchema = new Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Schema>
            {
                ["query"] = new Schema { Type = Google.GenAI.Types.Type.String }
            },
            Required = new List<string> { "query" }
        };

        var sqlSchema = new Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Schema>
            {
                ["sql"] = new Schema { Type = Google.GenAI.Types.Type.String }
            },
            Required = new List<string> { "sql" }
        };

        var getDate = new FunctionDeclaration
        {
            Name = "GetDate",
            Description = "Get today's date",
            Parameters = noParamsSchema
        };

        var getTime = new FunctionDeclaration
        {
            Name = "GetTime",
            Description = "Get the current time",
            Parameters = noParamsSchema
        };

        var tavilySearch = new FunctionDeclaration
        {
            Name = "TavilySearch",
            Description = "Search the web and return results for a query",
            Parameters = tavilySchema
        };

        var getSchema = new FunctionDeclaration
        {
            Name = "GetSchema",
            Description = "Get the structure of the SQL database",
            Parameters = noParamsSchema
        };

        var retrieveTable = new FunctionDeclaration
        {
            Name = "RetrieveTable",
            Description = "Run a SELECT query on the SQL database and return the result as JSON",
            Parameters = sqlSchema
        };

        var executeNonQuery = new FunctionDeclaration
        {
            Name = "ExecuteNonQuery",
            Description = "Run INSERT, UPDATE, or DELETE on the SQL database " +
            "and return the number of affected rows",
            Parameters = sqlSchema
        };

        var timeDateTool = new Tool
        {
            FunctionDeclarations = [getDate, getTime]
        };

        var searchTool = new Tool
        {
            FunctionDeclarations = [tavilySearch]
        };

        var sqlTool = new Tool
        {
            FunctionDeclarations = [getSchema, retrieveTable, executeNonQuery]
        };

        var codeExecutionTool = new Tool
        {
            CodeExecution = new ToolCodeExecution()
        };

        return new List<Tool> { timeDateTool, searchTool, sqlTool, codeExecutionTool };
    }

    private static void SaveImage(byte[] bytes, string mime)
    {
        var ext = mime.Split('/').Last();
        var fileName = $"plot_{DateTime.Now:yyyyMMdd_HHmmss_fff}.{ext}";
        var folder = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Plots"));
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, fileName);
        System.IO.File.WriteAllBytes(fullPath, bytes);
        Console.WriteLine($"[Image saved: {fullPath}]");
    }


}
