using DotNetEnv;

// Automatically finds .env by searching up the directory tree
Env.TraversePath().Load();

//var chatService = new Gemini_SDK("gemini-2.5-flash");
//var chatService = new OpenAI_SDK("gpt-5-mini");
var chatService = new OpenAI_SDK_Response("gpt-5-mini");

while (true)
{
    // User prompt message
    Console.Write(">> ");
    string userMessage = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userMessage)) {break;}

    // Send the user's message to the chat model and await the response
    var result = await chatService.Call(userMessage);

    Console.WriteLine(result);
}

