#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

public class GPT_Edit_Image
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        Console.WriteLine("Enter file to edit fro folder Img/:(robot_tennis_2.png): ");
        var fileName = Console.ReadLine();
        var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Img", fileName));
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

        Console.WriteLine($"Image to edit: {filePath} \n");
        Console.Write("Edit prompt: ");
        var editPrompt = Console.ReadLine();
        var fileId = await openai.UploadImage(filePath);
        var editItems = new List<ResponseItem>
        {
            OpenAI_Tools.CreateUploadedImageItem(fileId),
            ResponseItem.CreateUserMessageItem(editPrompt)
        };

        var response = await openai.Call(editItems);
        //await openai.DeleteUploadedImage(fileId);
        Console.WriteLine($"\n{response.GetOutputText()}\n");
    }
}