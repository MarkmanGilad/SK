using DotNetEnv;
using Google.GenAI.Types;
using System.Text.Json;

public class Gemini_server_tools
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                You may use the Google Search grounding tool when needed.
                Use it especially for current events, recent news, and changing information.
                """;

        var googleSearchTool = new Tool
        {
            GoogleSearch = new GoogleSearch()
        };

        var gemini = new Gemini_Tools(
            model: "gemini-3.1-pro-preview",
            systemPrompt: systemPrompt,
            tools: new List<Tool> { googleSearchTool });

        while (true)
        {
            Console.Write("Ask: ");
            var question = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(question))
            {
                return;
            }

            var response = await gemini.Call(question);

            //PrintResponse(response);

            Console.WriteLine();
            var textPart = response.Candidates?[0].Content?.Parts?.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Text));
            Console.WriteLine(textPart?.Text ?? string.Empty);
            Console.WriteLine();
        }
    }

    private static void PrintResponse(GenerateContentResponse response)
    {
        Console.WriteLine("---- Response Details ----");

        if (response.Candidates is null) return;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content?.Parts is null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                Console.WriteLine($"Part Type: Text={part.Text is not null}, FunctionCall={part.FunctionCall is not null}");
            }

            // Print grounding metadata (search queries + sources)
            if (candidate.GroundingMetadata is not null)
            {
                Console.WriteLine();
                Console.WriteLine("-- Grounding Metadata --");
                Console.WriteLine(JsonSerializer.Serialize(candidate.GroundingMetadata, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
        }

        Console.WriteLine("--------------------------");
    }
}