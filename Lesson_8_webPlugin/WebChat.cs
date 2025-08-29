using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Tavily;
using Microsoft.SemanticKernel.Data;   // TavilyTextSearchOptions
using DotNetEnv;

namespace Lesson_8_webPlugin
{
    public class WebChat
    {
        private Kernel _kernel;
        private IChatCompletionService _chatService;
        private OpenAIPromptExecutionSettings _settings;
        private ChatHistory _history;

        public WebChat()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            
            string model = "gpt-4.1-mini";
            
            var TAVILY_API_KEY = Environment.GetEnvironmentVariable("TAVILY_API_KEY");

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(model, OpenAIKey);

            // Add Tavily as a plugin named "Web" with a Search function

            #pragma warning disable SKEXP0050 
            var tavily = new TavilyTextSearch(
                apiKey: TAVILY_API_KEY,
                options: new TavilyTextSearchOptions {SearchDepth = TavilySearchDepth.Advanced});
            #pragma warning restore SKEXP0050

            builder.Plugins.Add(tavily.CreateWithGetTextSearchResults("Tavily"));


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
                string string_builder = "";
                var stream = _chatService.GetStreamingChatMessageContentsAsync(_history, _settings, _kernel);
                await foreach (var chunk in stream)
                {
                    Console.Write(chunk);
                    string_builder += chunk.Content;
                }
                Console.WriteLine();
                _history.AddAssistantMessage(string_builder);
            }
        }
    }
}
    
