using DotNetEnv;
using Google.GenAI.Types;

public class Gemini_Create_Image
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                You may use the image generation tool to create images from the user's description.
                When the user asks to create an image, generate it based on the prompt.
                Whenever you create an image you MUST also return a text explanation.
                Always include one text part describing the image.
                IMPORTANT: You MUST always provide a brief text description of the image you created. 
                Do not return only the image data.
                """;

        var gemini = new Gemini_Images(
            model: "gemini-3.1-flash-image-preview",
            systemPrompt: systemPrompt);

        Console.Write("Create image prompt: ");
        var createPrompt = Console.ReadLine();

        var response = await gemini.Call(createPrompt);

        Console.WriteLine($"\n{Gemini_Images.GetOutputText(response)}\n");
        //Console.WriteLine("If an image was created, it was saved to the Img folder.");
    }
}
