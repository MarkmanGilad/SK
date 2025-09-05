using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToAudio;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


namespace Lesson_9_images_Voice
{
    public class Voice_generator
    {
        private Kernel _kernel;
        private ITextToAudioService _textToAudioService;

        public Voice_generator()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

            string audioModel = "tts-1"; // or "tts-1-hd" for higher quality
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAITextToAudio(
                apiKey: OpenAIKey,
                modelId: audioModel,
                serviceId: "t2a");  // text to audio

            _kernel = builder.Build();
            _textToAudioService = _kernel.GetRequiredService<ITextToAudioService>();
        }
        public async Task chat()
        {
            Console.InputEncoding = Encoding.UTF8;  // for Hebrew input
            Console.Write("What text do you want to convert to voice? >> ");
            string? userMessage = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userMessage)) { return; }

            var audioSettings = new OpenAITextToAudioExecutionSettings
            {
                Voice = "nova",                    // Available: alloy, echo, fable, onyx, nova, shimmer
                ResponseFormat = "mp3",             // mp3, opus, aac, flac
                Speed = 1.0f                        // 0.25 to 4.0
            };

            Console.WriteLine("working ....");

            var generated = await _textToAudioService.GetAudioContentAsync(
                text: userMessage,                  
                executionSettings: audioSettings,
                kernel: _kernel);

            var audio = generated;
            string generated_file = @"C:\Users\Gilad\source\repos\SK\Lesson_9_images\Audio\generated_voice.mp3";

            // Save audio file (similar to image WriteToFile)
            byte[] audioBytes = audio.Data.Value.ToArray();
            await System.IO.File.WriteAllBytesAsync(generated_file, audioBytes);
            Console.WriteLine("Generated: " + generated_file);
        }
    }
}
