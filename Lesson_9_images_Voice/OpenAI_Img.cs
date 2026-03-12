#pragma warning disable OPENAI001

using OpenAI.Images;
using DotNetEnv;

public class OpenAI_Img
{   
    private readonly ImageClient _imageClient;
    private readonly string _imgFolder;

    public OpenAI_Img(string model = "gpt-image-1")
    {
        Env.TraversePath().Load();
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
        _imageClient = new ImageClient(model, apiKey);
        _imgFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Img");
    }

    public async Task<byte[]> GenerateImageAsync(string prompt, string fileName = "generated.png")
    {
        var options = new ImageGenerationOptions
        {
            Size = GeneratedImageSize.W1024xH1024,
            Quality = GeneratedImageQuality.HighQuality,
            Background = GeneratedImageBackground.Transparent,
            OutputFileFormat = GeneratedImageFileFormat.Png
        };

        var result = await _imageClient.GenerateImageAsync(prompt, options);
        byte[] imageBytes = result.Value.ImageBytes.ToArray();

        string savePath = Path.Combine(_imgFolder, fileName);
        await File.WriteAllBytesAsync(savePath, imageBytes);
        Console.WriteLine($"Generated: {Path.GetFullPath(savePath)}");
        return imageBytes;
    }

    
}
