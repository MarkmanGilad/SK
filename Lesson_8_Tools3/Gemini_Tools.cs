using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;

public class Gemini_Tools
{
    private readonly Client GeminiModel;
    private readonly string Model;
    private readonly List<Content> history = new();
    private readonly string? systemPrompt;
    private readonly List<Tool>? tools;

    public Gemini_Tools(string model, string? systemPrompt = null, List<Tool>? tools = null)
    {
        GeminiModel = new Client();
        Model = model;
        this.systemPrompt = systemPrompt;
        this.tools = tools;
    }

    public Task<GenerateContentResponse> Call(string userMessage)
    {
        var newItems = new List<Content>
        {
            new Content { Role = "user", Parts = [ new Part { Text = userMessage } ] }
        };

        return Call(newItems);
    }

    public async Task<GenerateContentResponse> Call(List<Content> newItems)
    {
        foreach (var item in newItems)
        {
            history.Add(item);
        }

        var config = CreateConfig();

        var response = await GeminiModel.Models.GenerateContentAsync(model: Model, contents: history, config: config);

        // Save assistant text (tool flow will be handled by caller)
        if (response.Candidates.Count > 0 && response.Candidates[0].Content is not null)
        {
            history.Add(response.Candidates[0].Content);
        }

        return response;
    }

    private GenerateContentConfig CreateConfig()
    {
        var config = new GenerateContentConfig();

        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            config.SystemInstruction = new Content { Parts = [ new Part { Text = systemPrompt } ] };
        }

        if (tools is not null)
        {
            config.Tools = tools;
        }

        return config;
    }
}
