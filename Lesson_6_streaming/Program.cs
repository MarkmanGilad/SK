// https://platform.openai.com/settings/organization/general

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using DotNetEnv;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

string model = "gpt-4.1-mini"; //"gpt-5-mini";

// Create a Semantic Kernel builder instance
var builder = Kernel.CreateBuilder();

// Add the OpenAI chat completion service to the kernel builder
builder.AddOpenAIChatCompletion(model, OpenAIKey);

// Build the kernel with the configured services
var kernel = builder.Build();

// Retrieve the chat completion service from the kernel
var chatService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant");


while (true)
{
    Console.Write(">> ");
    string userMessage = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userMessage)) {break;}
    history.AddUserMessage(userMessage);
       
    string string_builder = "";

    var stream = chatService.GetStreamingChatMessageContentsAsync(
        chatHistory: history);

    await foreach (var chunk in stream)
    {
        Console.Write(chunk);
        string_builder += chunk.Content;
    }

    Console.WriteLine();
    history.AddAssistantMessage(string_builder);
}

