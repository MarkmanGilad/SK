
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

string GeminiAPIKey = "AIzaSyBPsnKZ8jJZHzxuhR3ZuDQ1rEF8ztQeWv8";
string model = "gemini-2.5-flash"; 

// Create a Semantic Kernel builder instance
var builder = Kernel.CreateBuilder();

// Add the OpenAI chat completion service to the kernel builder
builder.AddGoogleAIGeminiChatCompletion(model, GeminiAPIKey);

// Build the kernel with the configured services
var kernel = builder.Build();

// Retrieve the chat completion service from the kernel
var chatService = kernel.GetRequiredService<IChatCompletionService>();

// User prompt message
Console.Write("You >> ");
string userMessage = Console.ReadLine();

// Send the user's message to the chat model and await the response
var result = await chatService.GetChatMessageContentAsync(userMessage);

Console.WriteLine(result);


