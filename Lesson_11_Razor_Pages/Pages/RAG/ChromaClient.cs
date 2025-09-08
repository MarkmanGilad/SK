using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0020
// docker run --name chroma -p 8000:8000 -d chromadb/chroma:0.4.24  - we need Old chromaDB version until sk will update

namespace Lesson_11_Razor_Pages.Pages.RAG
{
    public class ChromaClient
    {
        private readonly Kernel _kernel;
        private readonly ChromaMemoryStore _store;
        private readonly string _collection;
        private static int _globalDocumentCounter = 0; // Static counter to ensure unique IDs across all uploads

        public ChromaClient(string openAiKey, string collectionName = "rag_demo",
                            string embeddingModel = "text-embedding-3-small",
                            string chromaUrl = "http://localhost:8000")
        {
            // Create AI kernel
            _kernel = Kernel.CreateBuilder()
                .AddOpenAIEmbeddingGenerator(embeddingModel, openAiKey)
                .Build();

            // Create database connection to ChromaDB
            _store = new ChromaMemoryStore(chromaUrl);
            _collection = collectionName;

            // Ensure the collection exists (create if missing)
            _store.CreateCollectionAsync(_collection).GetAwaiter().GetResult();
        }

        public async Task AddDocuments(List<string> documents, string? sourceFileName = null)
        {
            foreach (string document in documents)
            {
                // Skip empty documents
                if (string.IsNullOrWhiteSpace(document))
                    continue;

                // Create unique ID 
                string documentId = Guid.NewGuid().ToString();

                // Convert text to numbers (embeddings) using AI
                var embedding = await _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>()
                    .GenerateAsync(document);

                // Create a record to store in the database
                var record = MemoryRecord.LocalRecord(
                    id: documentId,
                    text: document,
                    description: sourceFileName != null ? $"From: {sourceFileName}" : null,
                    embedding: embedding.Vector);

                // Save to database (collection created automatically if it doesn't exist)
                await _store.UpsertAsync(_collection, record);
            }
        }

        public async Task<List<string>> Search(string question, int maxResults = 4)
        {

            // Convert the search question to vector (embeddings)
            var questionEmbedding = await _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>()
                .GenerateAsync(question);

            // Search the database for similar documents
            var foundDocuments = new List<(MemoryRecord Record, double Score)>();

            var searchResults = _store.GetNearestMatchesAsync(_collection, questionEmbedding.Vector,
                                limit: maxResults, minRelevanceScore: 0.0, withEmbeddings: false);

            await foreach (var result in searchResults)
            {
                foundDocuments.Add(result);
            }


            // Extract just the text from each result
            var sortedDocuments = new List<string>();
            foreach (var doc in foundDocuments)
            {
                sortedDocuments.Add(doc.Record.Metadata.Text);
            }
            return sortedDocuments;
        }

        public async Task DeleteCollection()
        {
            await _store.DeleteCollectionAsync(_collection);
        }

        public async Task ClearCollection()
        {
            await _store.DeleteCollectionAsync(_collection);
            await _store.CreateCollectionAsync(_collection);
            _globalDocumentCounter = 0; // Reset counter when collection is cleared
        }

    }
}
#pragma warning restore SKEXP0001
#pragma warning restore SKEXP0010
#pragma warning restore SKEXP0020

