using DotNetEnv;
using Lesson_1_2_http;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

var client = new OpenAIClient(apiKey, "gpt-4o-mini");
var history = new ChatHistory();

history.AddSystemMessage("You are a helpful assistant.");

Console.Write("You >> ");
var userMessage = Console.ReadLine();

history.AddUserMessage(userMessage);

var completion = await client.GetCompletionAsync(history);

Console.WriteLine($"OpenAI >> {completion}");

client.Dispose();
