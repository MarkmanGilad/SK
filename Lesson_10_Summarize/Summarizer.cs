
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Lesson_10_Summarize
{
    public class Summarizer
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly OpenAIPromptExecutionSettings _settings;
        private readonly string _language;
        private readonly string OPEN_AI_KEY;

        public Summarizer(string language = "Hebrew")
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

        public async Task<string> Summarize(string pdfPath, int pagesPerChunk = 5, int groupSize = 5)
        {
            Console.WriteLine("ExtractPages...");
            // Extract pages from pdf
            var pageTexts = ExtractPages(pdfPath); 
            
            // make pages chunk
            var windows = MakePageWindows(pageTexts, pagesPerChunk);

            // MAP: summarize each window using a dedicated function
            Console.WriteLine($"Mapping {pageTexts.Count} pages...");
            var partials = new List<string>(windows.Count);
            foreach (var page in windows)
            {
                var summary = await MapWindowAsync(page);
                partials.Add(summary);
            }
            Console.WriteLine($"Reduce {partials.Count} summaries...");
            
            // REDUCE: merge summaries hierarchically until one remains
            var final = await ReduceManyAsync(partials, groupSize);
            return final;
        }

        // --- MAP function ---
        private async Task<string> MapWindowAsync(string text)
        {
            var mapSystem = $"""
                You are summarizing a partial text segment.
                Output detailed and comprehensive bullet points; keep all important facts, names, numbers, and dates.
                If significant context is missing, mention this in one sentence.
                Write the summarized page numbers at the start of the text: [pages 3-7]
                Write your summary in {_language}.
                """;
            var history = new ChatHistory(mapSystem);
            history.AddUserMessage(text);
            var result = await _chat.GetChatMessageContentAsync(history, _settings, _kernel);
            return result.Content ?? string.Empty;
        }

        // ---- ExtractPages functio
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

        // ---------- Build chunks with one ovelapping page ----------
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

        // ---------- Reduce (hierarchical) ----------
        private async Task<string> ReduceManyAsync(List<string> parts, int groupSize)
        {
            while (parts.Count > 1)
            {
                var next = new List<string>((parts.Count + groupSize - 1) / groupSize);
                for (int i = 0; i < parts.Count; i += groupSize)
                {
                    int count = Math.Min(groupSize, parts.Count - i);
                    var batch = parts.GetRange(i, count);
                    var merged = await ReduceOnceAsync(batch);
                    next.Add(merged);
                }
                parts = next;
            }
            return parts[0];
        }

        private async Task<string> ReduceOnceAsync(List<string> parts)
        {
            var reduceSystem = $"""
                Combine the following bullet summaries into a detailed, comprehensive summary in {_language}.
                - Remove duplicates and contradictions
                - Keep key facts, entities, dates, numbers
                - Output 10–20 bullet points, then a 4–6 sentence abstract
            """;

            var history = new ChatHistory(reduceSystem);
            history.AddUserMessage(string.Join("\n\n---\n\n", parts));
            var reply = await _chat.GetChatMessageContentAsync(history, _settings, _kernel);
            return reply.Content ?? string.Empty;
        }
    }
}
