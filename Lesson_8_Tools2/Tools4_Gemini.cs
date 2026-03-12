using Google.GenAI.Types;

public class Tools4_Gemini
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

        var tool = new Tool
        {
            FunctionDeclarations = [ getDate, getTime, tavilySearch ]
        };

        var gemini = new Gemini_Tools(
            model: "gemini-3.1-pro-preview",
            systemPrompt: systemPrompt,
            tools: new List<Tool> { tool });

        Console.Write("Ask your question: ");
        var message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) return;

        const int maxSteps = 5;

        var response = await gemini.Call(message);

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
                    else
                    {
                        toolResult = "Unknown tool: " + call.Name;
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
                var text = response.Candidates[0].Content.Parts[0].Text;
                Console.WriteLine(text);
                return;
            }

            if (step == maxSteps - 1)
            {
                toolOutputMessage.Parts.Add(new Part
                {
                    Text = "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."
                });

                response = await gemini.Call(new List<Content> { toolOutputMessage });

                var finalText = response.Candidates[0].Content.Parts[0].Text;
                Console.WriteLine(finalText);
                return;
            }

            response = await gemini.Call(new List<Content> { toolOutputMessage });
        }

        Console.WriteLine("Max iterations reached.");
    }
}
