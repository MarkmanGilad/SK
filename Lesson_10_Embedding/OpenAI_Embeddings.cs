using DotNetEnv;
using OpenAI.Embeddings;

internal class OpenAI_Embeddings
{
    private readonly EmbeddingClient _embeddingClient;
    private readonly string _embeddingModel = "text-embedding-3-small";

    public OpenAI_Embeddings()
    {
        Env.TraversePath().Load();
        string openAiKey = Environment.GetEnvironmentVariable("OpenAIKey")!;

        _embeddingClient = new EmbeddingClient(_embeddingModel, openAiKey);
    }

    public async Task<float[]> EmbedAsync(string paragraph)
    {
        OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(paragraph);
        return embedding.ToFloats().ToArray();
    }

    public double CosineSimilarity(float[] v1, float[] v2)
    {
        if (v1.Length != v2.Length)
        {
            throw new ArgumentException("Embedding vectors must have the same length.");
        }

        double dot = 0;
        double normA = 0;
        double normB = 0;

        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            normA += v1[i] * v1[i];
            normB += v2[i] * v2[i];
        }

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}