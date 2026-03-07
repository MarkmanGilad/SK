using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace Lesson_9_images 
{
    

    public class Img_generator
    {
        private Kernel _kernel;
        private ITextToImageService _textToImageService;
       
        public Img_generator()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            
            string imgModel = "dall-e-3"; // "gpt-image-1"; 
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAITextToImage(
                apiKey: OpenAIKey,
                modelId: imgModel,
                serviceId: "t2i");  // text to image

            _kernel = builder.Build();
            _textToImageService = _kernel.GetRequiredService<ITextToImageService>();

        }
        
        public async Task chat()
        {
            string systemPrompt = "You are an image creator. Create the the following image:";
            Console.Write("What Image Do you want to generate ? >>  ");
            string userMessage = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userMessage)) { return; }

            string combinedPrompt = $"{systemPrompt}. {userMessage}";
            
            var imageSettings = new OpenAITextToImageExecutionSettings
            {
                Size = (1024, 1024),            // Image size as a tuple (Width, Height)
                Quality = "hd",                 // Quality: "standard" or "hd" (high definition) - only dall-e
                Style = "vivid",                // Style: "vivid" or "natural" - only dall-e
                ResponseFormat = "b64_json"     // "url" or "b64_json" - only dall-e
            };
            
            Console.WriteLine("working ....");

            var generated = await _textToImageService.GetImageContentsAsync(
                input: combinedPrompt, executionSettings: imageSettings, kernel: _kernel);

            var gen = generated[0];
            string generated_file = @"C:\Users\Gilad\source\repos\SK\Lesson_9_images\Img\generated_4.jpg";
            gen.WriteToFile(generated_file, overwrite: true);
            Console.WriteLine("Generated: " + generated_file);

        }
    }
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
