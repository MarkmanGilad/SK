using System.Text.Json;

public class Tools1_GPT
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

        var stepSchema = """
            {
              "type": "object",
              "properties": {
                "thought": { "type": "string" },
                "action": { "type": "string", "enum": ["GetDate", "FinalAnswer"] },
                "input": { "type": "string" }
              },
              "required": ["thought", "action", "input"],
              "additionalProperties": false
            }
            """;

        var tools = new DateTimeTools();
        var gpt = new OpenAI_SDK_Response(model: "gpt-5.2", systemPrompt: systemPrompt);
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        Console.Write("Ask your question: ");
        var userQuestion = Console.ReadLine();

        var stepJson = await gpt.Call(userQuestion, schema: stepSchema);
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
            var message = $"Tool result: {toolResult}. Now return FinalAnswer with the result.";
            var finalJson = await gpt.Call(message, stepSchema);

            var finalStep = JsonSerializer.Deserialize<AgentStep>(finalJson, jsonOptions);

            Console.WriteLine(finalStep.Input);
        }
    }
}

