using Google.GenAI;
using Google.GenAI.Types;
using DotNetEnv;

public class Google_Voice
{
    private readonly Client _client;
    private readonly string _audioFolder;
    private string Model;

    public Google_Voice(string model = "gemini-2.5-flash-preview-tts")
    {
        Env.TraversePath().Load();

        var apiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");
        _client = new Client(apiKey: apiKey);
        _audioFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Audio");
        Model = model;
    }

    public async Task<byte[]> GenerateVoiceAsync(
        string prompt, string fileName = "generated.wav", string voiceName = "Kore")
    {
        var config = new GenerateContentConfig
        {
            ResponseModalities = new List<string> { "AUDIO" },
            SpeechConfig = new SpeechConfig
            {
                VoiceConfig = new VoiceConfig
                {
                    PrebuiltVoiceConfig = new PrebuiltVoiceConfig
                    {
                        VoiceName = voiceName
                    }
                }
            }
        };
        var response = await _client.Models.GenerateContentAsync(
            model: Model, contents: prompt, config: config);

        byte[] audioBytes = response.Candidates[0].Content.Parts[0].InlineData.Data.ToArray();

        string savePath = Path.Combine(_audioFolder, fileName);
        await System.IO.File.WriteAllBytesAsync(savePath, audioBytes);

        Console.WriteLine($"Generated: {Path.GetFullPath(savePath)}");
        return audioBytes;
    }
}
