using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Tavily;
using Microsoft.SemanticKernel.Data;   // TavilyTextSearchOptions
using DotNetEnv;
using System;
using System.Threading.Tasks;

#pragma warning disable SKEXP0050

namespace Lesson_8_webPlugin
{
    internal class WebChat_Gemini
    {
        private Kernel _kernel;
        private IChatCompletionService _chatService;
        private OpenAIPromptExecutionSettings _settings;
        private ChatHistory _history;

        // Reusing the DateTimePlugin from Plugin4/Plugin4_Gemini
        public class DateTimePlugin
        {
            [KernelFunction, System.ComponentModel.Description("Return today's date in a friendly format")]
            public string GetDate()
            {
                return DateTime.Now.ToString("dddd, MMMM dd, yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            [KernelFunction, System.ComponentModel.Description("Return the current time (HH:mm:ss)")]
            public string GetTime()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        public WebChat_Gemini()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var GeminiAPIKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            
            // Use Gemini 2.0 Flash via OpenAI-compatible endpoint
            string model = "gemini-2.5-flash";
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: GeminiAPIKey,
                endpoint: new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
            );

            // Add Tavily Search
            var TAVILY_API_KEY = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
            var tavily = new TavilyTextSearch(
                apiKey: TAVILY_API_KEY,
                options: new TavilyTextSearchOptions { SearchDepth = TavilySearchDepth.Advanced });
            builder.Plugins.Add(tavily.CreateWithGetTextSearchResults("Tavily"));
            
            // Add DateTime as a plugin 
            var dt_plugin = new DateTimePlugin();
            _kernel = builder.Build();
            _kernel.ImportPluginFromObject(dt_plugin, "DateTime");

            _chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            // Use ToolCallBehavior for better compatibility with Gemini/OpenAI adapter
            _settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            _history = new ChatHistory();
            // Simplified system message usually works better with OpenAI protocol.
            // Also explicitly mention the Search tool to encourage its use.
            _history.AddSystemMessage("You are a helpful assistant. Use local tools or Tavily Search for real-time information when needed.");
        }

        public async Task chat()
        {
            while (true)
            {
                Console.Write("Ask Gemini Web>> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }
                               
                _history.AddUserMessage(userMessage);

                // Non-streaming call needed for reliable tool use with Gemini
                var result = await _chatService.GetChatMessageContentAsync(_history, _settings, _kernel);
                
                Console.Clear();
                // Show all new messages added during the call (tool messages)
                foreach (var msg in _history) 
                {
                    if (msg.Role == AuthorRole.System)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{msg.Role}]: {msg.Content}");
                        Console.ResetColor();
                    }
                    else if (msg.Role == AuthorRole.Tool)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{msg.Role}]: {msg.Content}");
                        // Inspect for FunctionResultContent
                        if (msg.Items != null)
                        {
                            foreach (var item in msg.Items)
                            {
                                if (item is FunctionResultContent res)
                                {
                                     Console.ForegroundColor = ConsoleColor.Green;
                                     Console.WriteLine($" [Function Result] {res.Result?.ToString()?.Substring(0, Math.Min(res.Result?.ToString()?.Length ?? 0, 100))}..."); 
                                     Console.ResetColor();
                                }
                            }
                        }
                        Console.ResetColor();
                    }
                    else if (msg.Role == AuthorRole.User)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"[{msg.Role}]: {msg.Content}");
                        Console.ResetColor();
                    }
                    else if (msg.Role == AuthorRole.Assistant)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"[{msg.Role}]: {msg.Content}");
                        
                        // Check for tool calls
                        if (msg.Items != null)
                        {
                            foreach (var item in msg.Items)
                            {
                                if (item is FunctionCallContent call)
                                {
                                    Console.Write($" [Tool Call] {call.PluginName}.{call.FunctionName}()"); 
                                }
                            }
                        }
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }

                // Show the final assistant response again clearly
                Console.WriteLine("\n--- Final Response ---");
                Console.WriteLine($"Assistant: {result.Content}");

                // Add the assistant response to history
                _history.AddAssistantMessage(result.Content);
            }
        }
    }
}
