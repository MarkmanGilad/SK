using DotNetEnv;

Env.TraversePath().Load();

//var searchTool = new TavilySearch();
//var results = await searchTool.Search("Who is Gilad Markman from Israel?");
//Console.WriteLine(results);


// SQLClient
//var SQLClient = new SQLTools();
//var table = SQLClient.RetrieveTable("SELECT * FROM GRADES");
//Console.WriteLine(table);


// SQLTools


//var tools = new Tools_GPT();

//var tools = new Tools_GPT_Thinking();
var tools = new Tools_Gemini_Thinking();

await tools.Run(thinking: true);