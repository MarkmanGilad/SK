using DotNetEnv;

Env.TraversePath().Load();

var tool_example = new Tools1_GPT();

await tool_example.Run();
