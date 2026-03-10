#pragma warning disable OPENAI001
using OpenAI.Responses;
using System.Text.Json;

public class RagChat
{
    private readonly PineconeClient _pineconeClient = new();
    private readonly OpenAI_Tools _openai;

    public RagChat()
    {
        var systemPrompt = """
            You answer questions only from the indexed documents.
            Use PineconeSearch when you need to search the documents.
            If the answer is not found in the documents, say:
            "I don't have information about that in the documents."
            """;

        var pineconeSchema = BinaryData.FromString("""
            {
                "type":"object",
                "properties":{
                    "question":{"type":"string"}
                },
                "required":["question"],
                "additionalProperties":false
            }
            """);

        var pineconeTool = ResponseTool.CreateFunctionTool(
            functionName: "PineconeSearch",
            functionParameters: pineconeSchema,
            strictModeEnabled: true,
            functionDescription: "Search the Pinecone vector database and return relevant document chunks");

        _openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool> { pineconeTool });
    }

    public async Task<string> GetAnswer(string question)
    {
        const int maxSteps = 5;

        var response = await _openai.Call(question);

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

                    if (call.FunctionName == "PineconeSearch")
                    {
                        var argsStr = call.FunctionArguments.ToString();
                        var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsStr);

                        string searchQuestion = args["question"].GetString()!;
                        var docs = await _pineconeClient.Search(searchQuestion, 4);
                        toolResult = string.Join("\n\n", docs);
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
                return response.GetOutputText();
            }

            if (step == maxSteps - 1)
            {
                toolOutputs.Add(ResponseItem.CreateUserMessageItem(
                    "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));

                response = await _openai.Call(toolOutputs);
                return response.GetOutputText();
            }

            response = await _openai.Call(toolOutputs);
        }

        return "Max iterations reached.";
    }
}
