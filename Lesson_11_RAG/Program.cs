using DotNetEnv;

Env.TraversePath().Load();

Console.WriteLine("RAG System");
Console.WriteLine("=============");

Console.Write("Do you want to load and index a PDF? (Y/N): ");
var loadPdf = Console.ReadKey().KeyChar;
Console.WriteLine();

if (loadPdf == 'Y' || loadPdf == 'y')
{
    Console.Write("Do you want to delete existing collection first? (Y/N): ");
    var deleteCollection = Console.ReadKey().KeyChar;
    Console.WriteLine();

    var pdfLoader = new PdfLoader();
    var pineconeClient = new PineconeClient();

    if (deleteCollection == 'Y' || deleteCollection == 'y')
    {
        await pineconeClient.DeleteCollection();
        Console.WriteLine("Collection cleared");
    }

    Console.Write("Enter PDF file name: ");
    string fileName = Console.ReadLine()!;

    Console.WriteLine("Loading PDF...");

    var documents = pdfLoader.LoadParagraphs(fileName);
    Console.WriteLine($"Loaded {documents.Count} chunks");

    Console.WriteLine("Indexing documents...");
    await pineconeClient.AddDocuments(documents);
    Console.WriteLine("Documents indexed");
}

Console.WriteLine("\nRAG Chat Started!");
Console.WriteLine("Ask questions about your documents. Type 'quit' to exit.\n");

var ragChat = new RagChat();

while (true)
{
    Console.Write("You: ");
    var question = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(question) || question.ToLower() == "quit")
        break;

    var answer = await ragChat.GetAnswer(question);
    Console.WriteLine($"AI: {answer}\n");
}

Console.WriteLine("Goodbye!");
