using DotNetEnv;
using Lesson_14_Http;

class Program
{
    static async Task Main(string[] args)
    {
        //await OpenAIChat();
        await GeminiChat();
    }

    static async Task OpenAIChat()
    {
        Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

        using var client = new OpenAIClient(apiKey, "gpt-4o-mini");
        var history = new ChatHistory();

        history.AddSystemMessage("You are a helpful assistant.");

        while (true)
        {
            Console.Write("You (OpenAI)>> ");
            var userMessage = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userMessage)) { break; }

            history.AddUserMessage(userMessage);

            var completion = await client.GetResponseAsync(history);

            Console.WriteLine($"OpenAI>> {completion}");

            history.AddAssistantMessage(completion);
        }
    }

    static async Task GeminiChat()
    {
        Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
        var apiKey = Environment.GetEnvironmentVariable("GeminiAPIKey");

        using var client = new GeminiClient(apiKey, "gemini-2.0-flash-exp");
        var history = new ChatHistory();

        history.AddSystemMessage("You are a helpful assistant.");

        while (true)
        {
            Console.Write("You (Gemini)>> ");
            var userMessage = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userMessage)) { break; }

            history.AddUserMessage(userMessage);

            var completion = await client.GetCompletionAsync(history);

            Console.WriteLine($"Gemini>> {completion}");

            history.AddAssistantMessage(completion);
        }
    }
}



