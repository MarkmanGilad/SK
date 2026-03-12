using System.Text.Json;
using Google.GenAI.Types;

public class Tools2_Gemini
{
    public async Task Run()
    {
        var systemPrompt = """
                Always reply with JSON only:
                {
                  "Thought": "why you chose the action",
                  "Action": "GetDate" or "GetTime" or "FinalAnswer",
                  "Input": "" // empty for tool calls, or the final answer text for FinalAnswer
                }

                If you need today's date, use GetDate.
                If you need the current time, use GetTime.
                After a tool result is provided, return FinalAnswer with the result.
            """;

        var stepSchema = new Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Schema>
            {
                ["thought"] = new() { Type = Google.GenAI.Types.Type.String },
                ["action"] = new()
                {
                    Type = Google.GenAI.Types.Type.String,
                    Enum = new List<string> { "GetDate", "GetTime", "FinalAnswer" }
                },
                ["input"] = new() { Type = Google.GenAI.Types.Type.String }
            },
            Required = new List<string> { "thought", "action", "input" }
        };

        var gemini = new Gemini_SDK(model: "gemini-2.5-flash", systemPrompt: systemPrompt);
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var tools = new DateTimeTools();

        Console.Write("Ask your question: ");
        var userQuestion = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(userQuestion)) return;

        var message = userQuestion;
        const int maxSteps = 5;

        for (int i = 0; i < maxSteps; i++)
        {
            if (i == maxSteps - 1)
            {
                message = """
                    Max tool steps reached. No more tool calls are allowed. 
                    Return Action="FinalAnswer" and fill Input with your best final answer using the information you already have. 
                    If the answer is incomplete, say what is missing.";
                    """;
            }

            var stepJson = await gemini.Call(message, stepSchema);
            var step = JsonSerializer.Deserialize<AgentStep>(stepJson, jsonOptions);
            if (step is null) return;

            if (step.Action == "FinalAnswer")
            {
                Console.WriteLine(step.Input);
                return;
            }

            if (step.Action == "GetDate")
            {
                var toolResult = tools.GetDate();
                message = $"Tool result: {toolResult}. If you still need information choose a tool, otherwise return FinalAnswer.";
                continue;
            }

            if (step.Action == "GetTime")
            {
                var toolResult = tools.GetTime();
                message = $"Tool result: {toolResult}. If you still need information choose a tool, otherwise return FinalAnswer.";
                continue;
            }
        }
        Console.WriteLine("Max iterations reached without FinalAnswer.");
    }
}
