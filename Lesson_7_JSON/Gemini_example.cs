using System.Text.Json;
using Google.GenAI.Types;

public static class Gemini_example
{
    public static async Task Run()
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var gemini = new Gemini_SDK("gemini-2.5-flash");
        var userMessage = "List the 10 countries with the highest population and the population number.";
        
        // A) No schema - plain text output
        var a = await gemini.Call(userMessage);
        Console.WriteLine("\nA) No schema (text):");
        Console.WriteLine(a);

        // B) Schema in prompt only (not strict)
        //var b = await gemini.Call(userMessage + " answer as JSON array like [{\"name\":\"...\",\"population\":123}].");
        var b = await gemini.Call(
            userMessage + 
            " answer as JSON array like with name and population.");
        
        Console.WriteLine("\nB) Schema in prompt only (not strict):");
        Console.WriteLine(b);

        try
        {
            var bParsed = JsonSerializer.Deserialize<List<Country>>(b, jsonOptions);
            if (bParsed == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Prompt structure error (B): response could not be deserialized to List<Country>.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"B parsed count: {bParsed.Count}");
            }
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Prompt structure error (B): invalid JSON format. {ex.Message}");
            Console.ResetColor();
        }

        // C) Strict schema (object)
        var strictSchema = new Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Schema>
            {
                ["countries"] = new Schema
                {
                    Type = Google.GenAI.Types.Type.Array,
                    Items = new Schema
                    {
                        Type = Google.GenAI.Types.Type.Object,
                        Properties = new Dictionary<string, Schema>
                        {
                            ["name"] = new Schema { Type = Google.GenAI.Types.Type.String },
                            ["population"] = new Schema { Type = Google.GenAI.Types.Type.Number }
                        },
                        Required = new List<string> { "name", "population" }
                    }
                }
            },
            Required = new List<string> { "countries" }
        };

        var c = await gemini.Call(userMessage, strictSchema);
        Console.WriteLine("\nC) Strict schema (object):");
        Console.WriteLine(JsonSerializer.Serialize(JsonDocument.Parse(c).RootElement, new JsonSerializerOptions { WriteIndented = true }));

        try
        {
            var cParsed = JsonSerializer.Deserialize<CountriesResponse>(c, jsonOptions);
            if (cParsed?.Countries == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Prompt structure error (C): response could not be deserialized to CountriesResponse.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"C parsed count: {cParsed.Countries.Count}");
            }
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Prompt structure error (C): invalid JSON format. {ex.Message}");
            Console.ResetColor();
        }
    }
}
