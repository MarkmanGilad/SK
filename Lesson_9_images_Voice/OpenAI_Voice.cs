#pragma warning disable OPENAI001

using OpenAI.Audio;
using DotNetEnv;

public class OpenAI_Voice
{
    private readonly AudioClient _audioClient;
    private readonly string _audioFolder;

    public OpenAI_Voice(string model = "tts-1")
    {
        Env.TraversePath().Load();
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
        _audioClient = new AudioClient(model, apiKey);
        _audioFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Audio");
    }

    public async Task<byte[]> GenerateVoiceAsync(string prompt, string fileName = "generated.mp3")
    {
        var options = new SpeechGenerationOptions
        {
            ResponseFormat = GeneratedSpeechFormat.Mp3,
            Instructions = "Speak in a warm, clear teaching tone."
        };

        var result = await _audioClient.GenerateSpeechAsync(
            prompt, GeneratedSpeechVoice.Shimmer, options);

        byte[] audioBytes = result.Value.ToArray();

        string savePath = Path.Combine(_audioFolder, fileName);
        await System.IO.File.WriteAllBytesAsync(savePath, audioBytes);
        Console.WriteLine($"Generated: {Path.GetFullPath(savePath)}");

        return audioBytes;
    }
}
