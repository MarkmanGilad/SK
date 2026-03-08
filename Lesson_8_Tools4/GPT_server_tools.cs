#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;
using System.Text.Json;

public class GPT_server_tools
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                You may use the OpenAI server web search tool when needed.
                Use it especially for current events, recent news, and changing information.
                """;

        var webSearchTool = ResponseTool.CreateWebSearchTool();

        var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool> { webSearchTool });

        while (true)
        {
            Console.Write("Ask: ");
            var question = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(question))
            {
                return;
            }

            var response = await openai.Call(question);

            //PrintOutputItems(response);

            Console.WriteLine();
            Console.WriteLine(response.GetOutputText());
            Console.WriteLine();
        }
    }

    private static void PrintOutputItems(ResponseResult response)
    {
        Console.WriteLine("---- OutputItems ----");

        foreach (var item in response.OutputItems)
        {
            Console.WriteLine(item.GetType().Name);
            Console.WriteLine(JsonSerializer.Serialize(item, item.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            Console.WriteLine();
        }

        Console.WriteLine("---------------------");
    }
}