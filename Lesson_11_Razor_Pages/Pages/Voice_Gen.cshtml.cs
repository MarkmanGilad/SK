using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToAudio;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

namespace Lesson_11_Razor_Pages.Pages
{
    public class Voice_GenModel : PageModel
    {
        [BindProperty]
        public string UserPrompt { get; set; } = string.Empty;

        [BindProperty]
        public string Voice { get; set; } = "nova";

        [BindProperty]
        public string Format { get; set; } = "mp3";

        [BindProperty]
        public float Speed { get; set; } = 1.0f;

        public string? GeneratedAudioPath { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsGenerating { get; set; }

        public void OnGet()
        {
            // Initialize default values if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(UserPrompt))
            {
                ErrorMessage = "Please enter text to convert to voice.";
                return Page();
            }

            try
            {
                IsGenerating = true;

                Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
                var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

                if (string.IsNullOrEmpty(OpenAIKey))
                {
                    ErrorMessage = "OpenAI API key not found. Please check your .env file.";
                    return Page();
                }

                string audioModel = "tts-1";
                var builder = Kernel.CreateBuilder();

                builder.AddOpenAITextToAudio(
                    apiKey: OpenAIKey,
                    modelId: audioModel,
                    serviceId: "t2a");

                var kernel = builder.Build();
                var textToAudioService = kernel.GetRequiredService<ITextToAudioService>();

                var audioSettings = new OpenAITextToAudioExecutionSettings
                {
                    Voice = Voice,
                    ResponseFormat = Format,
                    Speed = Speed
                };

                var generated = await textToAudioService.GetAudioContentAsync(
                    text: UserPrompt,
                    executionSettings: audioSettings,
                    kernel: kernel);

                var audio = generated;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"generated_voice_{timestamp}.{Format}";
                string fullPath = Path.Combine("wwwroot", "audio", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                byte[] audioBytes = audio.Data.Value.ToArray();
                await System.IO.File.WriteAllBytesAsync(fullPath, audioBytes);

                GeneratedAudioPath = $"/audio/{fileName}";
                SuccessMessage = $"Audio generated successfully! Prompt: \"{UserPrompt}\"";
                IsGenerating = false;
            }
            catch (Exception ex)
            {
                IsGenerating = false;
                ErrorMessage = $"Error generating audio: {ex.Message}";
            }

            return Page();
        }
    }
}
