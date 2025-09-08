using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Lesson_11_Razor_Pages.Pages.RAG;
using DotNetEnv;

namespace Lesson_11_Razor_Pages.Pages
{
    public class RAGModel : PageModel
    {
        [BindProperty]
        public IFormFile? PdfFile { get; set; }

        public string UserMessage { get; set; } = string.Empty;

        public ChatHistory History { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public bool IsProcessing { get; set; }
        public bool IsDocumentLoaded { get; set; }
        public List<string> LoadedDocuments { get; set; } = new();

        public void OnGet()
        {
            LoadHistoryFromSession();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            LoadHistoryFromSession();
            
            if (PdfFile == null || PdfFile.Length == 0)
            {
                ErrorMessage = "Please select a PDF file to upload.";
                return Page();
            }

            if (!PdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = "Please upload a PDF file only.";
                return Page();
            }

            var fileName = Path.GetFileName(PdfFile.FileName);

            try
            {
                // Load API key from .env file
                Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
                var openAiKey = Environment.GetEnvironmentVariable("OpenAIKey");
                
                if (string.IsNullOrEmpty(openAiKey))
                {
                    ErrorMessage = "OpenAI API key is not configured in .env file.";
                    return Page();
                }

                // Save uploaded file to wwwroot/uploads
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PdfFile.CopyToAsync(stream);
                }

                // Initialize components
                var pdfLoader = new PdfLoader("wwwroot/uploads");
                var chromaClient = new ChromaClient(openAiKey, "pdf_documents");

                // Load and process PDF
                var documents = pdfLoader.LoadParagraphs(fileName);
                
                if (!documents.Any())
                {
                    ErrorMessage = "No text could be extracted from the PDF file.";
                    return Page();
                }

                // Index documents (will append to existing documents)
                await chromaClient.AddDocuments(documents, fileName);

                SuccessMessage = $"✅ PDF '{fileName}' uploaded and indexed successfully! {documents.Count} chunks processed.";
                
                // Clean up uploaded file
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error processing PDF '{fileName}': {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChatAsync()
        {
            LoadHistoryFromSession();
            
            var userMessage = Request.Form["UserMessage"].ToString();
            
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return Page();
            }

            try
            {
                // Load API key from .env file
                Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
                var openAiKey = Environment.GetEnvironmentVariable("OpenAIKey");
                
                if (string.IsNullOrEmpty(openAiKey))
                {
                    ErrorMessage = "OpenAI API key is not configured.";
                    return Page();
                }

                // Create new RAG chat each time
                var ragChat = new RagChat(openAiKey, "pdf_documents");
                
                // Let RagChat handle everything - it will add messages to history
                var answer = await ragChat.GetAnswer(userMessage, History);

                SaveHistoryToSession();
                UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error getting response: {ex.Message}";
                UserMessage = string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostClearCollectionAsync()
        {
            LoadHistoryFromSession();
            
            try
            {
                // Load API key from .env file
                Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
                var openAiKey = Environment.GetEnvironmentVariable("OpenAIKey");
                
                if (!string.IsNullOrEmpty(openAiKey))
                {
                    var chromaClient = new ChromaClient(openAiKey, "pdf_documents");
                    await chromaClient.ClearCollection();
                }
                
                SuccessMessage = "✅ Document collection cleared successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error clearing collection: {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnPostClearChat()
        {
            History = new ChatHistory();
            History.AddSystemMessage("I am an AI assistant designed to help answer questions based on provided documents when available, or provide general assistance when no documents are loaded.");
            SaveHistoryToSession();
            return Page();
        }

        private class ChatMessage
        {
            public string Role { get; set; }
            public string Message { get; set; }
        }

        private void SaveHistoryToSession()
        {
            var list = new List<ChatMessage>();
            foreach (var msg in History)
            {
                list.Add(new ChatMessage { Role = msg.Role.ToString(), Message = msg.Content });
            }
            var result = System.Text.Json.JsonSerializer.Serialize(list);
            HttpContext.Session.SetString("RAGHistory", result);
        }

        private void LoadHistoryFromSession()
        {
            var json = HttpContext.Session.GetString("RAGHistory");
            string system_prompt = "I am an AI assistant designed to help answer questions based on provided documents when available, or provide general assistance when no documents are loaded.";
            
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
                else if (item.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    History.AddAssistantMessage(item.Message);
                else if (item.Role.Equals("system", StringComparison.OrdinalIgnoreCase))
                    History.AddSystemMessage(item.Message);
            }
        }
    }
}
