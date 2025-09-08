using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace Lesson_12_RAG
{
    public class PdfLoader
    {
        private readonly string _dataDir;

        public PdfLoader(string? dataDir = null)
        {
            if (dataDir != null)
            {
                _dataDir = dataDir;
            }
            else
            {
                // Navigate from bin output directory back to project root, then to Data folder
                var baseDir = AppContext.BaseDirectory;
                var projectRoot = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;
                _dataDir = Path.Combine(projectRoot ?? baseDir, "Data");
            }
        }

        public List<string> LoadParagraphs(string fileName)
        {
            var path = Path.Combine(_dataDir, fileName);
            var text = ExtractText(path);
            return SplitIntoChunks(text, 500); // 500 characters per chunk
        }

        private static string ExtractText(string path)
        {
            var sb = new StringBuilder();
            using var reader = new PdfReader(path);
            using var document = new PdfDocument(reader);
            
            for (int i = 1; i <= document.GetNumberOfPages(); i++)
            {
                var page = document.GetPage(i);
                var pageText = PdfTextExtractor.GetTextFromPage(page);
                sb.AppendLine(pageText);
                sb.AppendLine(); // Blank line between pages
            }
            
            return sb.ToString();
        }

        private static List<string> SplitIntoChunks(string text, int chunkSize = 500)
        {
            var chunks = new List<string>();
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var currentChunk = new StringBuilder();
            
            foreach (var sentence in sentences)
            {
                // If adding this sentence would make the chunk too long, start a new chunk
                if (currentChunk.Length > 0 && currentChunk.Length + sentence.Length > chunkSize)
                {
                    chunks.Add(currentChunk.ToString().Trim());
                    currentChunk.Clear();
                }
                
                if (currentChunk.Length > 0) currentChunk.Append(" ");
                currentChunk.Append(sentence);
            }
            
            // Add the last chunk
            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().Trim());
            }
            
            return chunks.Where(c => c.Length > 50).ToList(); // Filter out very short chunks
        }
    }
}

