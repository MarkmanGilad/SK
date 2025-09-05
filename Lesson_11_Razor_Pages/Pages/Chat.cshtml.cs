using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Tavily;
using Microsoft.SemanticKernel.Data;   // TavilyTextSearchOptions
using DotNetEnv;
using System.Runtime;
using System.ComponentModel;
#pragma warning disable SKEXP0050

namespace Lesson_11_Razor_Pages.Pages
{
    public class ChatModel : PageModel
    {
        [BindProperty]
        public string UserMessage { get; set; }
        public ChatHistory History{ get; set; } = new();
        public string Direction { get; set; } = "ltr";

        public void OnGet()
        {
            LoadHistoryFromSession();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("direction")))
            {
                Direction = "ltr";
            }
            else
            {
                Direction = HttpContext.Session.GetString("direction");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadHistoryFromSession();
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string model = "gpt-4.1-mini";
            var TAVILY_API_KEY = Environment.GetEnvironmentVariable("TAVILY_API_KEY");

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(model, OpenAIKey);
            var tavily = new TavilyTextSearch(
                apiKey: TAVILY_API_KEY,
                options: new TavilyTextSearchOptions { SearchDepth = TavilySearchDepth.Basic });
            builder.Plugins.Add(tavily.CreateWithGetTextSearchResults("Tavily"));
            //builder.Plugins.AddFromType<DateTimePlugin>("DateTime");
            var kernel = builder.Build();
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
            History.AddUserMessage(UserMessage);
            var result = await chatService.GetChatMessageContentAsync(History, settings, kernel);
            History.AddAssistantMessage(result.Content);
            SaveHistoryToSession();
            return Page();
        }

        private class ChatMessage
        {
            public string Role { get; set; }
            public string Message { get; set; }
        }

        public class DateTimePlugin
        {
            [KernelFunction("get_date")]
            [Description("Get today's date")]
            [return: Description("Return date format DD-MM-YYYY.")]
            public string Today()
            {
                return DateTime.Now.ToString("dd-MM-yyyy");
            }
            [KernelFunction("get_time")]
            [Description("Get Current time")]
            public string Current_time()
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        private void SaveHistoryToSession()
        {
            var list = new List<ChatMessage>();
            foreach (var msg in History)
            {
                list.Add(new ChatMessage { Role = msg.Role.ToString(), Message = msg.Content });
            }
            var result = System.Text.Json.JsonSerializer.Serialize(list);
            HttpContext.Session.SetString("History", result);
        }

        private void LoadHistoryFromSession()
        {
            var json = HttpContext.Session.GetString("History");
            string system_prompt = """
                You are a helpful assistant. 
                Use Tavily.Search when you need fresh info and cite links.
                """;
            if (string.IsNullOrEmpty(json))
            {
                History.AddSystemMessage(system_prompt);
                return;
            }

            var list = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(json);
            foreach (var item in list)
            {
                if (item.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    History.AddUserMessage(item.Message);
                else
                    History.AddAssistantMessage(item.Message);
            }
            
        }

        public IActionResult OnPostToggleDirection()
        {
            Direction = HttpContext.Session.GetString("direction") ?? "ltr";
            Direction = Direction == "ltr" ? "rtl" : "ltr";
            HttpContext.Session.SetString("direction", Direction);
            LoadHistoryFromSession();
            return Page();
        }



    }
}
