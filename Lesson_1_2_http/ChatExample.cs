using DotNetEnv;

namespace Lesson_1_2_http;

public class ChatExample
{
    public static async Task Main(string[] args)
    {
        Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

        var client = new OpenAIClient(apiKey, "gpt-4o-mini");
        var history = new ChatHistory();
        
        history.AddSystemMessage("You are a helpful assistant.");

        while (true)
        {
            Console.Write(">> ");
            var userMessage = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                break; // Exit the loop if the user enters an empty message
            }

            history.AddUserMessage(userMessage);
            
            var completion = await client.GetResponseAsync(history);
            
            Console.WriteLine(completion);
            
            history.AddAssistantMessage(completion);
        }
        
        client.Dispose();
    }
}
