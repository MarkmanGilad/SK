using DotNetEnv;

Env.TraversePath().Load();

//var tool_example = new Tools1_GPT();  // one tool call, no agent loop
//var tool_example = new Tools1_Gemini(); // one tool call, no agent loop
//var tool_example = new Tools2_GPT(); // multiple tool calls with agent loop
//var tool_example = new Tools2_Gemini(); // multiple tool calls with agent loop
//var tool_example = new Tools3_GPT();
var tool_example = new Tools3_Gemini();

await tool_example.Run();
