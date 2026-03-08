using DotNetEnv;

Env.TraversePath().Load();

//var searchTool = new TavilySearch();
//var results = await searchTool.Search("Who is Gilad Markman from Israel?");
//Console.WriteLine(results);

var tools = new Tools_GPT();
//var tools = new Tools_Gemini();

await tools.Run();