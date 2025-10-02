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

        public PdfLoader()
        {
            // Navigate from bin output directory back to project root, then to Data folder
            var baseDir = AppContext.BaseDirectory;
            var projectRoot = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;
            _dataDir = Path.Combine(projectRoot ?? baseDir, "Data");

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
            
            string[] sentences = Regex.Split(text, @"(?<=[.!?])\s+");

            var current = new StringBuilder();

            foreach (var raw in sentences)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var sentence = raw.Trim();
                
                if (current.Length > 0 ) current.Append(" ");
                current.Append(sentence);

                // If adding this sentence would overflow, flush current first
                if (current.Length > chunkSize)
                {
                    chunks.Add(current.ToString());
                    current.Clear();
                }
            }

            if (current.Length > 0)
                chunks.Add(current.ToString());

            return chunks;
        }
    }
}

