using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using DotNetEnv;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

string model = "gpt-5-mini"; //"gpt-4.1-mini";

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
    // User prompt message
    Console.Write(">> ");
    string userMessage = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userMessage)) {break; }

    history.AddUserMessage(userMessage);
    // Send the user's message to the chat model and await the response
    var result = await chatService.GetChatMessageContentAsync(history);

    Console.WriteLine(result.Content);
    history.AddAssistantMessage(result.Content);
}

