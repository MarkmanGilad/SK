using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Lesson_10_Summarize
{
    internal class RefineSummarizer
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly OpenAIPromptExecutionSettings _settings;
        private readonly string _language;
        private readonly string OPEN_AI_KEY;

        public RefineSummarizer(string language = "Hebrew")
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            OPEN_AI_KEY = Environment.GetEnvironmentVariable("OpenAIKey");
            string modelId = "gpt-4.1-mini";

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(apiKey: OPEN_AI_KEY, modelId: modelId, serviceId: "chat");
            _kernel = builder.Build();
            _chat = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings { Temperature = 0.2 };
            _language = language;

        }

        public async Task<string> Summarize(string pdfPath, int pagesPerChunk = 5)
        {
            Console.WriteLine("ExtractPages...");
            // Extract pages from pdf
            var pageTexts = ExtractPages(pdfPath);
            
            // make pages chunk
            var windows = MakePageWindows(pageTexts, pagesPerChunk);

            Console.WriteLine($"Refining summary for {pageTexts.Count} pages...");
            
            // REFINE: iteratively update summary with each chunk
            string currentSummary = string.Empty;
            for (int i = 0; i < windows.Count; i++)
            {
                Console.WriteLine($"Processing chunk {i + 1}/{windows.Count}...");
                currentSummary = await RefineWithChunkAsync(currentSummary, windows[i]);
            }
            
            return currentSummary;
        }

        // --- REFINE function ---
        private async Task<string> RefineWithChunkAsync(string existingSummary, string newChunk)
        {
            var refineSystem = $"""
                You are updating an existing summary with new information from a text chunk.
                If the existing summary is empty, create a new summary from the chunk.
                If the existing summary exists, integrate the new information while:
                - Keeping all important facts, names, numbers, and dates
                - Removing duplicates and contradictions
                - Maintaining chronological order when possible
                - Writing the summarized page numbers at the start: [pages X-Y]
                Write your summary in {_language}.
                """;
            
            var history = new ChatHistory(refineSystem);
            
            if (string.IsNullOrEmpty(existingSummary))
            {
                history.AddUserMessage($"Create a summary from this text chunk:\n\n{newChunk}");
            }
            else
            {
                history.AddUserMessage($"Existing summary:\n{existingSummary}\n\nNew chunk to integrate:\n{newChunk}");
            }
            
            var result = await _chat.GetChatMessageContentAsync(history, _settings, _kernel);
            return result.Content ?? string.Empty;
        }

        // ---- ExtractPages function
        private static List<string> ExtractPages(string path)
        {
            var pages = new List<string>();
            using var pdf = PdfDocument.Open(path);
            foreach (var page in pdf.GetPages())
            {
                var txt = ContentOrderTextExtractor.GetText(page) ?? string.Empty;
                pages.Add(txt);
            }
            return pages; // index 0 == page 1
        }

        // ---------- Build chunks with one overlapping page ----------
        private static List<string> MakePageWindows(List<string> pageTexts, int pagesPerChunk)
        {
            var windows = new List<string>();
            string chunk = $"[Page 1]\n{pageTexts[0]}\n";
            for (int i = 1; i < pageTexts.Count; i++)
            {
                int pageNo = i + 1;
                chunk += $"[Page {pageNo}]\n{pageTexts[i]}\n";

                if (pageNo % pagesPerChunk == 0 || pageNo == pageTexts.Count)
                {
                    windows.Add(chunk);
                    chunk = $"[Page {pageNo}]\n{pageTexts[i]}\n";
                }
            }
            return windows;
        }
    }
}
