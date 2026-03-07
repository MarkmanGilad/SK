

public class Tools1_Gemini
{
    public async Task Run()
    {
        var systemPrompt = """
            Always reply with JSON only:
            {
              "Thought": "why you chose the action",
              "Action": "GetDate" or "FinalAnswer",
              "Input": "" // empty for GetDate, or the final answer text for FinalAnswer
            }

            If you need today's date to answer, set Action="GetDate" and leave Input empty.
            If you can answer without tools, set Action="FinalAnswer" and put the full answer in Input.
            """;

        var stepSchema = new Google.GenAI.Types.Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Google.GenAI.Types.Schema>
            {
                ["thought"] = new() { Type = Google.GenAI.Types.Type.String },
                ["action"] = new()
                {
                    Type = Google.GenAI.Types.Type.String,
                    Enum = new List<string> { "GetDate", "FinalAnswer" }
                },
                ["input"] = new() { Type = Google.GenAI.Types.Type.String }
            },
            Required = new List<string> { "thought", "action", "input" }
        };

        var tools = new DateTimeTools();
        var gemini = new Gemini_SDK("gemini-2.5-flash", systemPrompt);
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        Console.Write("Ask your question: ");
        var userQuestion = Console.ReadLine();
                       
        var stepJson = await gemini.Call(userQuestion, stepSchema);
        var step = System.Text.Json.JsonSerializer.Deserialize<AgentStep>(stepJson, jsonOptions);
        
        if (step.Action == "FinalAnswer")
        {
            Console.WriteLine(step.Input);
            return;
        }

        if (step.Action == "GetDate")
        {
            var toolResult = tools.GetDate();
            var message = $"Tool result: {toolResult}. Now return FinalAnswer with the result.";
            var finalJson = await gemini.Call(message, stepSchema);
            
            var finalStep = System.Text.Json.JsonSerializer.Deserialize<AgentStep>(finalJson, jsonOptions);
            
            Console.WriteLine(finalStep.Input);
        }
    }
}

