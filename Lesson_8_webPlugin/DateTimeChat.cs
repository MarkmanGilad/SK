using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;



namespace Lesson_8_webPlugin
{
    public class DateTimeChat
    {
        private Kernel _kernel;
        private IChatCompletionService _chatService;
        private OpenAIPromptExecutionSettings _settings;
        private ChatHistory _history;

        public DateTimeChat() 
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");

            string model = "gpt-4.1-mini"; //"gpt-5-mini";
                        
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(model, OpenAIKey);
            builder.Plugins.AddFromType<DateTimePlugin>("DateTime");
            _kernel = builder.Build();
            _chatService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            _history = new ChatHistory();
            _history.AddSystemMessage("You are a helpful assistant that always answer to the point");
        }

        public async Task ask ()
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
