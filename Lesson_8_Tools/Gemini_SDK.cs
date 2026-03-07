using Google.GenAI;
using Google.GenAI.Types;

public class Gemini_SDK
{
    private readonly Client GeminiModel;
    private readonly string Model;
    private readonly List<Content> history = new();
    private readonly string? systemPrompt;

    public Gemini_SDK(string model, string? systemPrompt = null)
    {
        GeminiModel = new Client();
        Model = model;
        this.systemPrompt = systemPrompt;
    }

    public async Task<string> Call(string userMessage, Schema? responseSchema = null)
    {
        history.Add(new Content {Role = "user",Parts = [new Part { Text = userMessage }]});

        var config = new GenerateContentConfig();

        if (responseSchema is not null)
        {
            config.ResponseMimeType = "application/json";
            config.ResponseSchema = responseSchema;
        }

        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            config.SystemInstruction = new Content { Parts = [new Part { Text = systemPrompt }] };
        }

        var response = await GeminiModel.Models.GenerateContentAsync(
            model: Model, contents: history, config: config
        );
        var text = response.Candidates[0].Content.Parts[0].Text;
        history.Add(new Content {Role = "model", Parts = [new Part { Text = text }]});
        return text;
    }
}