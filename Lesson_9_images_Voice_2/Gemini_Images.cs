using Google.GenAI;
using Google.GenAI.Types;
using GeminiFile = Google.GenAI.Types.File;

public class Gemini_Images
{
    private readonly Client GeminiModel;
    private readonly Files FileModel;
    private readonly List<Content> history = new();
    private readonly string model;
    private readonly string? systemPrompt;
    private readonly List<Tool>? tools;
    private readonly List<string>? responseModalities;
    private readonly string imageFolderPath;

    public Gemini_Images(
        string model,
        string? systemPrompt = null,
        List<Tool>? tools = null,
        List<string>? responseModalities = null,
        string imageFolderName = "Img")
    {
        var geminiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");

        if (string.IsNullOrWhiteSpace(geminiKey))
        {
            throw new InvalidOperationException("GeminiAPIKey environment variable was not found.");
        }

        GeminiModel = new Client(apiKey: geminiKey);
        FileModel = GeminiModel.Files;
        this.model = model;
        this.systemPrompt = systemPrompt;
        this.tools = tools;
        this.responseModalities = responseModalities ?? [Modality.Text.ToString(), Modality.Image.ToString()];

        imageFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", imageFolderName));
        Directory.CreateDirectory(imageFolderPath);
    }

    public Task<GenerateContentResponse> Call(string userMessage)
    {
        return Call(
        [
            new Content
            {
                Role = "user",
                Parts = [new Part { Text = userMessage }]
            }
        ]);
    }

    public async Task<GenerateContentResponse> Call(List<Content> newItems)
    {
        foreach (var item in newItems)
        {
            history.Add(item);
        }

        var config = CreateConfig();

        var response = await GeminiModel.Models.GenerateContentAsync(model: model, contents: history, config: config);

        if (response.Candidates.Count > 0 && response.Candidates[0].Content is not null)
        {
            history.Add(response.Candidates[0].Content);
        }

        SaveGeneratedImages(response);

        return response;
    }

    private GenerateContentConfig CreateConfig()
    {
        var config = new GenerateContentConfig();

        if (responseModalities is not null)
        {
            config.ResponseModalities = responseModalities;
        }

        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            config.SystemInstruction = new Content
            {
                Parts = [new Part { Text = systemPrompt }]
            };
        }

        if (tools is not null)
        {
            config.Tools = tools;
        }

        return config;
    }

    public void ClearHistory()
    {
        history.Clear();
    }

    public async Task<GeminiFile> UploadImage(string filePath)
    {
        return await FileModel.UploadAsync(filePath, new UploadFileConfig
        {
            MimeType = GetMimeType(filePath),
            DisplayName = Path.GetFileName(filePath)
        });
    }

    public static string GetOutputText(GenerateContentResponse response)
    {
        var textParts = new List<string>();

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content?.Parts is null)
            {
                continue;
            }

            foreach (var part in candidate.Content.Parts)
            {
                if (!string.IsNullOrWhiteSpace(part.Text))
                {
                    textParts.Add(part.Text.Trim());
                }
            }
        }

        return string.Join(System.Environment.NewLine, textParts);
    }

    private void SaveGeneratedImages(GenerateContentResponse response)
    {
        foreach (var candidate in response.Candidates)
        {
            foreach (var part in candidate.Content?.Parts ?? [])
            {
                if (part.InlineData?.Data is null ||
                    string.IsNullOrWhiteSpace(part.InlineData.MimeType) ||
                    !part.InlineData.MimeType.StartsWith("image/"))
                {
                    continue;
                }

                var extension = GetFileExtension(part.InlineData.MimeType);
                var filePath = Path.Combine(imageFolderPath, $"{Guid.NewGuid()}{extension}");
                System.IO.File.WriteAllBytes(filePath, part.InlineData.Data);
            }
        }
    }

    private static string GetMimeType(string filePath)
    {
        switch (Path.GetExtension(filePath).ToLowerInvariant())
        {
            case ".png":
                return "image/png";

            case ".jpg":
            case ".jpeg":
                return "image/jpeg";

            case ".webp":
                return "image/webp";

            case ".gif":
                return "image/gif";

            default:
                return "application/octet-stream";
        }
    }

    private static string GetFileExtension(string mimeType)
    {
        switch (mimeType.ToLowerInvariant())
        {
            case "image/png":
                return ".png";

            case "image/jpeg":
                return ".jpg";

            case "image/webp":
                return ".webp";

            case "image/gif":
                return ".gif";

            default:
                return ".bin";
        }
    }
}
