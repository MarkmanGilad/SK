#pragma warning disable OPENAI001
using OpenAI.Responses;

public class RagChat
{
    private readonly OpenAI_Tools _openai;

    public RagChat(string? vectorStoreId = null)
    {
        var systemPrompt = """
            You answer questions only from the uploaded document.
            Use the file search tool when you need to search the uploaded document.
            If the answer is not found in the document, say:
            "I don't have information about that in the documents."
            """;

        var tools = new List<ResponseTool>();

        if (!string.IsNullOrWhiteSpace(vectorStoreId))
        {
            tools.Add(ResponseTool.CreateFileSearchTool(vectorStoreIds: new[] { vectorStoreId }));
        }

        _openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: tools);
    }

    public async Task<string> GetAnswer(string question)
    {
        var response = await _openai.Call(question);
        return response.GetOutputText();
    }
}
