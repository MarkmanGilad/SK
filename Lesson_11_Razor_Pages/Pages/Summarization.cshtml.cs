using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;
using UglyToad.PdfPig;  //install NuGet pdfpig
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Lesson_11_Razor_Pages.Pages
{
    public class SummarizationModel : PageModel
    {
        [BindProperty]
        public IFormFile? PdfFile { get; set; }

        [BindProperty]
        public int PagesPerChunk { get; set; } = 5;

        [BindProperty]
        public int GroupSize { get; set; } = 5;

        [BindProperty]
        public string Language { get; set; } = "English";

        public string? SummaryResult { get; set; }
        public string? SummaryFilePath { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsSummarizing { get; set; }

        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly OpenAIPromptExecutionSettings _settings;

        public SummarizationModel()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var OpenAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string modelId = "gpt-4.1-mini";

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(apiKey: OpenAIKey, modelId: modelId, serviceId: "chat");
            _kernel = builder.Build();
            _chat = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings { Temperature = 0.2 };
        }

        public void OnGet()
        {
            // Initialize default values if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PdfFile == null || PdfFile.Length == 0)
            {
                ErrorMessage = "Please select a PDF file to summarize.";
                return Page();
            }

            if (!PdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = "Please upload a valid PDF file.";
                return Page();
            }

            try
            {
                IsSummarizing = true;

                // Save uploaded PDF
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"uploaded_pdf_{timestamp}.pdf";
                string docsPath = Path.Combine("wwwroot", "docs");
                Directory.CreateDirectory(docsPath);
                string pdfPath = Path.Combine(docsPath, fileName);

                using (var stream = new FileStream(pdfPath, FileMode.Create))
                {
                    await PdfFile.CopyToAsync(stream);
                }

                // Summarize the PDF
                var summary = await SummarizePdf(pdfPath, PagesPerChunk, GroupSize);

                // Save summary to text file
                string summaryFileName = $"summary_{timestamp}.txt";
                string summaryPath = Path.Combine(docsPath, summaryFileName);
                await System.IO.File.WriteAllTextAsync(summaryPath, summary);

                SummaryResult = summary;
                SummaryFilePath = $"/docs/{summaryFileName}";
                SuccessMessage = $"PDF summarized successfully! Original file: {PdfFile.FileName}";

                IsSummarizing = false;
            }
            catch (Exception ex)
            {
                IsSummarizing = false;
                ErrorMessage = $"Error summarizing PDF: {ex.Message}";
            }

            return Page();
        }

        private async Task<string> SummarizePdf(string pdfPath, int pagesPerChunk, int groupSize)
        {
            var pageTexts = ExtractPages(pdfPath);
            var windows = MakePageWindows(pageTexts, pagesPerChunk);

            // MAP: summarize each window
            var partials = new List<string>(windows.Count);
            foreach (var page in windows)
            {
                var summary = await MapWindowAsync(page);
                partials.Add(summary);
            }

            // REDUCE: merge summaries hierarchically
            var final = await ReduceManyAsync(partials, groupSize);
            return final;
        }

        private async Task<string> MapWindowAsync(string text)
        {
            var mapSystem = $"""
                    You are summarizing a partial text segment.
                    Output detailed and comprehensive bullet points; keep all important facts, names, numbers, and dates.
                    If significant context is missing, mention this in one sentence.
                    Write the summarized page numbers at the start of the text: [pages 3-7]
                    Write your summary in {Language}.
                    """;
            var h = new ChatHistory(mapSystem);
            h.AddUserMessage(text);
            var r = await _chat.GetChatMessageContentAsync(h, _settings, _kernel);
            return r.Content ?? string.Empty;
        }

        private static List<string> ExtractPages(string path)
        {
            var pages = new List<string>(256);
            using var pdf = PdfDocument.Open(path);
            foreach (var page in pdf.GetPages())
            {
                var txt = ContentOrderTextExtractor.GetText(page) ?? string.Empty;
                pages.Add(txt);
            }
            return pages;
        }

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

        private async Task<string> ReduceOnceAsync(IEnumerable<string> parts)
        {
            var reduceSystem = $"""
                    Combine the following bullet summaries into a detailed, comprehensive summary in {Language}.
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
