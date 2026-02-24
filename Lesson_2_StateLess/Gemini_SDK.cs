using Google.GenAI;

public class Gemini_SDK
{
    private readonly Client GeminiModel;
    private readonly string Model;

    public Gemini_SDK(string model)
    {
        GeminiModel = new Client();
        Model = model;
    }

    public async Task<string> Call(string userMessage)
    {
            
        // Send the request
        var response = await this.GeminiModel.Models.GenerateContentAsync(
            model: "gemini-2.5-flash", contents: userMessage
        );

        // Get the response content
        return response.Candidates[0].Content.Parts[0].Text;
    }
}
