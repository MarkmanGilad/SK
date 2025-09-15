using DotNetEnv;
using Lesson_7_plugin;

Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");

var plugin = new Plugin1();

await plugin.Run();
