using Google.GenAI;

public class Gemini_SDK
{
    private readonly Client GeminiModel;
    private readonly string Model;

    public Gemini_SDK(string model)
    {
        // Client() reads the Gemini API key from the GOOGLE_API_KEY environment variable.
        GeminiModel = new Client();
        Model = model;
    }

    public async Task<string> Call(string userMessage)
    {
            
        // Send the request
        var response = await GeminiModel.Models.GenerateContentAsync(
            model: Model, contents: userMessage
        );

        // Get the response content
        return response.Candidates[0].Content.Parts[0].Text;
    }
}
