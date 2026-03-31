using Google.GenAI;
using Google.GenAI.Types;

public class Gemini_SDK
{
    private readonly Client GeminiModel;
    private readonly string Model;
    private readonly List<Content> history = new();

    public Gemini_SDK(string model)
    {
        var apiKey = System.Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");
        }

        GeminiModel = new Client(apiKey: apiKey);
        Model = model;
    }

    public async Task<string> Call(string userMessage)
    {
        history.Add(new Content { Role = "user", Parts = [new Part { Text = userMessage }] });

        var response = await this.GeminiModel.Models.GenerateContentAsync(
            model: this.Model, contents: history);

        if (response.Candidates is null || response.Candidates.Count == 0)
        {
            throw new InvalidOperationException("Gemini returned no candidates.");
        }

        var content = response.Candidates[0].Content;

        if (content is null || content.Parts is null || content.Parts.Count == 0 || string.IsNullOrWhiteSpace(content.Parts[0].Text))
        {
            throw new InvalidOperationException("Gemini returned an empty response.");
        }

        var text = content.Parts[0].Text!;

        history.Add(new Content { Role = "model", Parts = [new Part { Text = text }] });
        return text;
    }
}
