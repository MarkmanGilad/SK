using DotNetEnv;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.TraversePath().Load();

        //await JSON_example.Run();
        await Gemini_example.Run();
        //await OpenAI_example.Run();
    }
}
