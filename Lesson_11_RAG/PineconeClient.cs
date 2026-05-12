using DotNetEnv;
using System.Net.Http.Json;
using System.Text.Json;

public class PineconeClient
{
    private readonly HttpClient _httpClient = new();
    private readonly OpenAI_Embeddings _embeddings = new();
    private readonly string _pinecodeApiKey;

    private readonly string _nameSpace = "rag";
    private readonly string _indexHost = "rag-h30zh6f.svc.aped-4627-b74a.pinecone.io";

    public PineconeClient()
    {
        Env.TraversePath().Load();
        _pinecodeApiKey = Environment.GetEnvironmentVariable("PINECODE_API")!;
    }

    public async Task AddDocuments(List<string> documents)
    {
        var vectors = new List<Dictionary<string, object?>>();

        foreach (string document in documents)
        {
            float[] embedding = await _embeddings.EmbedAsync(document);

            vectors.Add(new Dictionary<string, object?>
            {
                ["id"] = Guid.NewGuid().ToString(),
                ["values"] = embedding,
                ["metadata"] = new Dictionary<string, object?>
                {
                    ["text"] = document
                }
            });
        }

        var body = new Dictionary<string, object?>
        {
            ["namespace"] = _nameSpace,
            ["vectors"] = vectors
        };

        using var response = await PostAsync("vectors/upsert", body); 
    }

    public async Task<List<string>> Search(string question, int maxResults = 4)
    {
        float[] embedding = await _embeddings.EmbedAsync(question);

        var body = new Dictionary<string, object?>
        {
            ["namespace"] = _nameSpace,
            ["vector"] = embedding,
            ["topK"] = maxResults,
            ["includeMetadata"] = true,
            ["includeValues"] = false
        };
        using var response = await PostAsync("query", body);

        string response_str = await response.Content.ReadAsStringAsync();
        using JsonDocument json = JsonDocument.Parse(response_str);

        var results = new List<string>();
        JsonElement matches = json.RootElement.GetProperty("matches");

        foreach (JsonElement match in matches.EnumerateArray())
        {
            string text = match.GetProperty("metadata").GetProperty("text").GetString();
            results.Add(text);
        }

        return results;
    }

    public async Task ClearNameSpace()
    {
        var body = new Dictionary<string, object?>
        {
            ["namespace"] = _nameSpace,
            ["deleteAll"] = true
        };
        using var response = await PostAsync("vectors/delete", body);
    }

    private Task<HttpResponseMessage> PostAsync(string path, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_indexHost}/{path}");
        request.Headers.Add("Api-Key", _pinecodeApiKey);
        request.Content = JsonContent.Create(body);

        return _httpClient.SendAsync(request);
    }
}

