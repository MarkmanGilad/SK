using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using DotNetEnv;

#pragma warning disable SKEXP0010 
namespace Lesson_11._5_Embedding
{
    internal class EmbeddingDemo
    {
        // Create AI kernel
        private readonly Kernel _kernel;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly string _embeddingModel = "text-embedding-3-small";

        public EmbeddingDemo()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            string openAiKey = Environment.GetEnvironmentVariable("OpenAIKey")!;

            // Create AI kernel
            _kernel = Kernel.CreateBuilder()
                .AddOpenAIEmbeddingGenerator(_embeddingModel, openAiKey)
                .Build();

            _embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        }
        
        public async Task<Embedding<float>> EmbedAsync(string paragraph)
        {
            // Convert text to numbers (embeddings) using AI
            var embedding = await _embeddingGenerator.GenerateAsync(paragraph);
            return embedding;
        }

        public double CosineSimilarity(Embedding<float> v1, Embedding<float> v2)
        {
            var A = v1.Vector.ToArray();
            var B = v2.Vector.ToArray();

            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < A.Length; i++)
            {
                dot += A[i] * B[i];
                normA += A[i] * A[i];
                normB += B[i] * B[i];
            }
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        
    }
}

