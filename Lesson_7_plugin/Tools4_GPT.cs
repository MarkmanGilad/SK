#pragma warning disable OPENAI001
using OpenAI.Responses;

public class Tools4_GPT
{
    public async Task Run()
    {
        var gpt = new GPT_DateTime(
            model: "gpt-5.2");

        Console.Write("Ask your question: ");
        var message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) return;

        var answer = await gpt.Call(message);
        Console.WriteLine(answer);
    }
}
