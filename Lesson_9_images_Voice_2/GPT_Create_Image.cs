#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

public class GPT_Create_Image
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                You may use the image generation tool to create images from the user's description.
                When the user asks to create an image, generate it based on the prompt.
                After creating an image, always return a short text describing what you created.
                """;

        var imageGenerationTool = ResponseTool.CreateImageGenerationTool(model: "gpt-image-1");

        var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool> { imageGenerationTool });

        Console.Write("Create image prompt: ");
        var createPrompt = Console.ReadLine();

        var response = await openai.Call(createPrompt);

        Console.WriteLine($"\n{response.GetOutputText()}\n");
        //Console.WriteLine("If an image was created, it was saved to the Img folder.");
    }
}