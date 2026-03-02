using DotNetEnv;

// Automatically finds .env by searching up the directory tree
Env.TraversePath().Load();

Console.Write("Add System Prompt>> ");
string systemPrompt = Console.ReadLine();

//var chatService = new Gemini_SDK("gemini-3-flash-preview", systemPrompt);
//var chatService = new OpenAI_SDK("gpt-5-mini", systemPrompt);
var chatService = new OpenAI_SDK_Response("gpt-5.2", systemPrompt);

while (true)
{
    // User prompt message
    Console.Write(">> ");
    string userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage)) { break; }

    // Send the user's message to the chat model and await the response
    var result = await chatService.Call(userMessage);

    Console.WriteLine(result);
}

