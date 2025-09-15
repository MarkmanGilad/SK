using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;
using System.Runtime.InteropServices;
using DotNetEnv;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

string model = "gpt-4.1-mini";  //"gpt-3.5-turbo";  //"gpt-4.1-mini"; 

// Create a Semantic Kernel builder instance
var builder = Kernel.CreateBuilder();

// Add the OpenAI chat completion service to the kernel builder
builder.AddOpenAIChatCompletion(model, OpenAIKey);

// Build the kernel with the configured services
var kernel = builder.Build();

// Retrieve the chat completion service from the kernel
var chatService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory history = new ChatHistory();

while (true)
{
    Console.Write("System>> ");
    string systemMessage= Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(systemMessage))
    {
        history.AddSystemMessage(systemMessage);
    }
    Console.Write("User>> ");
    string userMessage = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userMessage)) {break;}

    history.AddUserMessage(userMessage);
    var result = await chatService.GetChatMessageContentAsync(history);
    Console.WriteLine(result.Content);
    history.Clear();
}

