using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;
using System.Runtime.InteropServices;
using DotNetEnv;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

string model = "gpt-4.1-mini";

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
//history.AddSystemMessage("You are an un helpful assistant that always answer with a question and never answer to the point");
//history.AddSystemMessage("You are an assistant that give a one sentence answer with Gilad Markman as the subject of this answer. Allways give Gilad Markman compliments in your answers");


var settings = new OpenAIPromptExecutionSettings {
    Temperature = 0.5,// creative max = 2
    MaxTokens = 20,
};

while (true)
{
    // User prompt message
    Console.Write("USER>> ");
    string userMessage = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userMessage)) {break; }
    
    Console.Write("Tokens>> ");
    int tokens = int.Parse(Console.ReadLine());
    settings.MaxTokens = tokens;

    history.AddUserMessage(userMessage);
    // Send the user's message to the chat model and await the response
    var result = await chatService.GetChatMessageContentAsync(history, settings, kernel);

    Console.WriteLine(result.Content);
    //history.AddAssistantMessage(result.Content);
    history.Clear();
}

