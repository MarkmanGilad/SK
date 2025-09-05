using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using DotNetEnv;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001 

namespace Lesson_11_Razor_Pages.Pages
{
    public class Image_GenModel : PageModel
    {
        [BindProperty]
        public string UserPrompt { get; set; } = string.Empty;

        [BindProperty]
        public string ImageSize { get; set; } = "1024x1024";

        [BindProperty]
        public string Quality { get; set; } = "hd";

        [BindProperty]
        public string Style { get; set; } = "vivid";

        public string? GeneratedImagePath { get; set; }
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
                ErrorMessage = "Please enter a description for the image you want to generate.";
                return Page();
            }

            try
            {
                IsGenerating = true;

                // Load environment variables
                Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
                var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

                if (string.IsNullOrEmpty(OpenAIKey))
                {
                    ErrorMessage = "OpenAI API key not found. Please check your .env file.";
                    return Page();
                }

                // Initialize Semantic Kernel 
                string imgModel = "dall-e-3";
                var builder = Kernel.CreateBuilder();

                builder.AddOpenAITextToImage(
                    apiKey: OpenAIKey,
                    modelId: imgModel,
                    serviceId: "t2i");

                var kernel = builder.Build();
                var textToImageService = kernel.GetRequiredService<ITextToImageService>();

                // Create system prompt (same as console app)
                string systemPrompt = "You are an image creator. Create the following image:";
                string combinedPrompt = $"{systemPrompt}. {UserPrompt}";

                // Parse image size
                var sizeParts = ImageSize.Split('x');
                int width = int.Parse(sizeParts[0]);
                int height = int.Parse(sizeParts[1]);

                // Create image settings (same as console app)
                var imageSettings = new OpenAITextToImageExecutionSettings
                {
                    Size = (width, height),
                    Quality = Quality,  // Quality: "standard" or "hd" (high definition) - only dall-e
                    Style = Style,      // Style: "vivid" or "natural" - only dall-e
                    ResponseFormat = "b64_json" // "url" or "b64_json" - only dall-e
                };

                // Generate image (same as console app)
                var generated = await textToImageService.GetImageContentsAsync(
                    input: combinedPrompt,
                    executionSettings: imageSettings,
                    kernel: kernel);

                var gen = generated[0];

                // Create unique filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"generated_image_{timestamp}.jpg";
                string fullPath = Path.Combine("wwwroot", "images", fileName);

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                // Save image (same as console app)
                gen.WriteToFile(fullPath, overwrite: true);

                // Set path for display (relative to wwwroot)
                GeneratedImagePath = $"/images/{fileName}";
                SuccessMessage = $"Image generated successfully! Prompt: \"{UserPrompt}\"";

                IsGenerating = false;
            }
            catch (Exception ex)
            {
                IsGenerating = false;
                ErrorMessage = $"Error generating image: {ex.Message}";
            }

            return Page();
        }
    }
}
