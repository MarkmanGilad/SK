using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lesson_12_RAG;
using DotNetEnv;

namespace Lesson_12_RAG
{
    public class ChromaClientTester
    {
        public ChromaClientTester() { }

        public async Task Run()
        {
            Console.WriteLine("ChromaDB + Semantic Kernel Test");

            // Load OpenAI API key from .env file
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            string openAiKey = Environment.GetEnvironmentVariable("OpenAIKey");

            if (string.IsNullOrEmpty(openAiKey))
            {
                Console.WriteLine("Error: OpenAI API key not found in .env file");
                return;
            }

            try
            {
                Console.WriteLine("Step 1: Creating ChromaClient with Semantic Kernel...");

                // Create ChromaClient with OpenAI key
                var client = new ChromaClient(openAiKey, collectionName: "test_collection");

                // Ask user if they want to clear the collection before starting
                Console.WriteLine("\nDo you want to clear the existing collection before adding new documents? (Y/N): ");
                var clearResponse = Console.ReadKey().KeyChar;
                Console.WriteLine(); // New line after key press

                if (clearResponse == 'Y' || clearResponse == 'y')
                {
                    Console.WriteLine("Clearing collection...");
                    await client.ClearCollection();
                    Console.WriteLine("Collection cleared successfully!");
                }

                Console.WriteLine("\nStep 2: Adding test documents...");

                // Test documents to add to the vector database
                var testDocuments = new List<string>
                {
                    "The cat sat on the mat. It was a sunny day.",
                    "Dogs are great pets and loyal companions.",
                    "Birds can fly high in the sky and sing beautiful songs.",
                    "Fish swim in the ocean and rivers.",
                    "Cats love to sleep in warm, sunny places."
                };

                // Add documents (this will create embeddings using OpenAI)
                await client.AddDocuments(testDocuments);

                Console.WriteLine("\nStep 3: Testing search functionality...");

                // Test searches
                string[] searchQueries =
                {
                    "What animals are mentioned?",
                    "Tell me about pets",
                    "Where do animals live?"
                };

                foreach (var query in searchQueries)
                {
                    Console.WriteLine($"\n Searching for: '{query}'");
                    var results = await client.Search(query, maxResults: 3);

                    Console.WriteLine($"Found {results.Count} results:");
                    for (int i = 0; i < results.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {results[i]}");
                    }
                }
                
                Console.WriteLine("\nChromaClient test completed successfully!");

                // Ask user if they want to delete the collection after testing
                Console.WriteLine("\nDo you want to delete the entire collection? (Y/N): ");
                var deleteResponse = Console.ReadKey().KeyChar;
                Console.WriteLine(); // New line after key press

                if (deleteResponse == 'Y' || deleteResponse == 'y')
                {
                    Console.WriteLine("Deleting collection...");
                    await client.DeleteCollection();
                    Console.WriteLine("Collection deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Collection kept for future runs.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"\nFull exception details:\n{ex}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
