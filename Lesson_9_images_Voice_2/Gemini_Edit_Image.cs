using DotNetEnv;
using Google.GenAI.Types;

public class Gemini_Edit_Image
{
    public async Task Run()
    {
        Console.Write("Image file name (soccer.png): ");
        var fileName = Console.ReadLine();
        var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Img", fileName));

        Env.TraversePath().Load();

        var systemPrompt = """
            You may use the image generation tool to edit images.
            When the user asks to edit an image, use the provided image and the prompt to create the edited version.
            Whenever you create an image you MUST also return a text explanation.
            Always include one text part describing the image.
            """;

        var gemini = new Gemini_Images(
            model: "gemini-3.1-flash-image-preview",
            systemPrompt: systemPrompt);

        Console.WriteLine($"Image to edit: {filePath}\n");
        Console.Write("Edit prompt: ");
        var editPrompt = Console.ReadLine();
        var uploadedFile = await gemini.UploadImage(filePath);
        var response = await gemini.Call(
        [
            new Content
            {
                Role = "user",
                Parts =
                [
                    Part.FromUri(uploadedFile.Uri, uploadedFile.MimeType),
                    new Part { Text = editPrompt }
                ]
            }
        ]);
        Console.WriteLine($"\n{Gemini_Images.GetOutputText(response)}\n");
    }
}
