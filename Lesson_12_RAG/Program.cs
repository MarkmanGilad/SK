using Lesson_12_RAG;
using Microsoft.SemanticKernel.ChatCompletion;
using DotNetEnv;

// Load API key
Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
string openAiKey = Environment.GetEnvironmentVariable("OpenAIKey")!;

Console.WriteLine("RAG System");
Console.WriteLine("=============");

// Ask if user wants to load PDF
Console.Write("Do you want to load and index a PDF? (Y/N): ");
var loadPdf = Console.ReadKey().KeyChar;
Console.WriteLine();

if (loadPdf == 'Y' || loadPdf == 'y')
{
    // Ask if user wants to delete collection first
    Console.Write("Do you want to delete existing collection first? (Y/N): ");
    var deleteCollection = Console.ReadKey().KeyChar;
    Console.WriteLine();

    // Initialize components
    var pdfLoader = new PdfLoader();
    var chromaClient = new ChromaClient(openAiKey, "pdf_documents");

    if (deleteCollection == 'Y' || deleteCollection == 'y')
    {
        await chromaClient.ClearCollection();
        Console.WriteLine("Collection cleared");
    }

    // Load and split PDF - hardcoded path
    Console.WriteLine("Loading PDF...");
    string path = "Thedangersofartificialintelligence.pdf"; // "Lucas A. Meyer - A Quick Tour of the Semantic Kernel.pdf";// "AI-2024-HEB.pdf"; 

    var documents = pdfLoader.LoadParagraphs(path);
    Console.WriteLine($"Loaded {documents.Count} chunks");

    // Embed and save to ChromaDB
    Console.WriteLine("Indexing documents...");
    await chromaClient.AddDocuments(documents);
    Console.WriteLine("Documents indexed");
}

// Start chat loop
Console.WriteLine("\nRAG Chat Started!");
Console.WriteLine("Ask questions about your documents. Type 'quit' to exit.\n");

var ragChat = new RagChat(openAiKey, "pdf_documents");
var chatHistory = new ChatHistory("You are a helpful assistant that answers questions based on provided documents.");

while (true)
{
    Console.Write("You: ");
    var question = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(question) || question.ToLower() == "quit")
        break;

    var answer = await ragChat.GetAnswer(question, chatHistory);
    Console.WriteLine($"AI: {answer}\n");
}

Console.WriteLine("Goodbye!");
