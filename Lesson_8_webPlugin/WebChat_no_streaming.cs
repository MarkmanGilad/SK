using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Tavily;
using Microsoft.SemanticKernel.Data;   // TavilyTextSearchOptions
using DotNetEnv;

#pragma warning disable SKEXP0050

namespace Lesson_8_webPlugin
{
    internal class WebChat_no_streaming
    {
        private Kernel _kernel;
        private IChatCompletionService _chatService;
        private OpenAIPromptExecutionSettings _settings;
        private ChatHistory _history;

        public WebChat_no_streaming()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

            string model = "gpt-4.1-mini";

            var TAVILY_API_KEY = Environment.GetEnvironmentVariable("TAVILY_API_KEY");

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(model, OpenAIKey);

            // Add Tavily as a plugin named "Web" with a Search function


            var tavily = new TavilyTextSearch(
                apiKey: TAVILY_API_KEY,
                options: new TavilyTextSearchOptions { SearchDepth = TavilySearchDepth.Advanced });
            builder.Plugins.Add(tavily.CreateWithGetTextSearchResults("Tavily"));

            builder.Plugins.AddFromType<DateTimePlugin>("DateTime");

            _kernel = builder.Build();
            _chatService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            _history = new ChatHistory();
            _history.AddSystemMessage("You are a helpful assistant. Use Tavily.Search when you need fresh info and cite links.");
        }

        public async Task chat()
        {
            while (true)
            {
                Console.Write(">> ");
                string userMessage = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userMessage)) { break; }
                               
                _history.AddUserMessage(userMessage);

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
                    if (msg.Role == AuthorRole.Tool)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{msg.Role}]: {msg.Content}");
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
                        string toolCalls = "";
                        try { 
                            toolCalls = $"ToolCalls: {((dynamic)msg).ToolCalls[0].FunctionArguments}"; 
                        } 
                        catch { }
                        Console.WriteLine($"[{msg.Role}]: {msg.Content} {toolCalls}");
                        // Add this anywhere in your foreach loop
                        
                        Console.ResetColor();
                    }
                    
                    // Skip System messages from display
                }

                // Show the final assistant response
                Console.WriteLine($"Assistant: {result.Content}");

                // Add the assistant response to history
                _history.AddAssistantMessage(result.Content);
            }
        }
    }
}

