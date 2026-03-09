#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

public class GPT_Create_Image
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                You may use the image generation tool to edit images.
                When the user asks to edit an image, use the provided image and the prompt to create the edited version.
                After editing an image, always return a short text explaining what you changed.
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