using DotNetEnv;

// Automatically finds .env by searching up the directory tree
Env.TraversePath().Load();

Console.Write("Add System Prompt>> ");
string systemPrompt = Console.ReadLine();

//var chatService = new Gemini_SDK("gemini-3-flash-preview", systemPrompt);
var chatService = new OpenAI_SDK_Response("gpt-5.2", systemPrompt);

while (true)
{
    // User prompt message
    Console.Write(">> ");
    string userMessage = Console.ReadLine();    

    if (string.IsNullOrWhiteSpace(userMessage)) { break; }

    // Stream response chunks from the chat model
    await foreach (var chunk in chatService.CallStream(userMessage))
    {
        Console.Write(chunk);
    }
    Console.WriteLine();
}

