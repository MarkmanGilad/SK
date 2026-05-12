using DotNetEnv;

Env.TraversePath().Load();

var tools = new Tools_GPT_Thinking();
//var tools = new Tools_Gemini_Thinking();

await tools.Run(thinking: true);