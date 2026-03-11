using DotNetEnv;

Env.TraversePath().Load();

Console.WriteLine("RAG System");
Console.WriteLine("=============");

OpenAI_Tools openAI;

Console.Write("Enter PDF file name: ");
string fileName = Console.ReadLine()!;

string path = Path.GetFullPath(Path.Combine(
    AppContext.BaseDirectory,
    "..", "..", "..",
    "Data",
    fileName));

Console.WriteLine("Uploading PDF...");

openAI = new OpenAI_Tools("gpt-5.2");
string fileId = await openAI.UploadFile(path);
string vectorStoreId = await openAI.CreateVectorStore(fileId);

Console.WriteLine("PDF uploaded");

Console.WriteLine("\nRAG Chat Started!");
Console.WriteLine("Ask questions about your documents. Type 'quit' to exit.\n");

var ragChat = new RagChat(vectorStoreId);

while (true)
{
    Console.Write("You: ");
    var question = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(question) || question.ToLower() == "quit")
        break;

    var answer = await ragChat.GetAnswer(question);
    Console.WriteLine($"AI: {answer}\n");
}

await openAI.DeleteVectorStore(vectorStoreId);
await openAI.DeleteUploadedFile(fileId);

Console.WriteLine("Goodbye!");
