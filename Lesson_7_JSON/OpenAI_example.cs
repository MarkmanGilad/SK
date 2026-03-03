using System.Text.Json;

public static class OpenAI_example
{
    public static async Task Run()
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var openai = new OpenAI_SDK_Response("gpt-5.2");
        var userMessage = "List the 10 countries with the highest population and the population number.";

        // A) No schema - plain text output
        var oa = await openai.Call(userMessage);
        Console.WriteLine("\nOA) OpenAI No schema (text):");
        Console.WriteLine(oa);

        // B) Schema in prompt only (not strict)
        var ob = await openai.Call(userMessage + " answer as JSON array like [{\"name\":\"...\",\"population\":123}].");
        Console.WriteLine("\nOB) OpenAI Schema in prompt only (not strict):");
        Console.WriteLine(ob);

        try
        {
            var obParsed = JsonSerializer.Deserialize<List<Country>>(ob, jsonOptions);
            if (obParsed == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Prompt structure error (OB): response could not be deserialized to List<Country>.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"OB parsed count: {obParsed.Count}");
            }
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Prompt structure error (OB): invalid JSON format. {ex.Message}");
            Console.ResetColor();
        }

        // C) Strict schema (string)
        var strictSchemaOpenAI = """
            {
                "type": "object",
                "properties": {
                    "countries": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "name": { "type": "string" },
                                "population": { "type": "number" }
                            },
                            "required": ["name", "population"],
                            "additionalProperties": false
                        }
                    }
                },
                "required": ["countries"],
                "additionalProperties": false
            }
            """;

        var oc = await openai.Call(userMessage, strictSchemaOpenAI);
        Console.WriteLine("\nOC) OpenAI Strict schema (string):");
        Console.WriteLine(JsonSerializer.Serialize(JsonDocument.Parse(oc).RootElement, new JsonSerializerOptions { WriteIndented = true }));

        try
        {
            var ocParsed = JsonSerializer.Deserialize<CountriesResponse>(oc, jsonOptions);
            if (ocParsed?.Countries == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Prompt structure error (OC): response could not be deserialized to CountriesResponse.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"OC parsed count: {ocParsed.Countries.Count}");
            }
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Prompt structure error (OC): invalid JSON format. {ex.Message}");
            Console.ResetColor();
        }
    }
}
