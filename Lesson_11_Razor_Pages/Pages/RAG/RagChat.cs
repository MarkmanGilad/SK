using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lesson_11_Razor_Pages.Pages.RAG
{
    public class RagChat
    {
        private readonly ChromaClient _chromaClient;
        private readonly IChatCompletionService _chatService;

        public RagChat(string openAiKey, string collectionName = "rag_documents")
        {
            // Initialize vector search
            _chromaClient = new ChromaClient(openAiKey, collectionName);

            // Initialize chat service
            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion("gpt-4.1-mini", openAiKey)
                .Build();

            _chatService = kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GetAnswer(string question, ChatHistory history)
        {
            // Step 1: Add the clean user question to history
            history.AddUserMessage(question);

            // Step 2: Search for relevant documents
            var relevantDocs = await _chromaClient.Search(question, maxResults: 3);

            // Step 3: Create context from found documents
            var context = string.Join("\n\n", relevantDocs);

            // Step 4: Add RAG context as system message for educational purposes
            var ragSystemMessage = $"""
                RAG Processing: I found relevant context from the uploaded documents to help answer this question.

                Context from documents:
                {context}

                Based on this context, I will now provide an answer. If the answer cannot be found in the provided context, I will indicate that the information is not available in the documents.
                """;

            history.AddSystemMessage(ragSystemMessage);

            // Step 5: Create a temporary history for the actual LLM call (without the educational system message)
            var tempHistory = new ChatHistory();
            
            // Copy all messages except the last system message we just added
            var messages = history.ToList();
            for (int i = 0; i < messages.Count - 1; i++)
            {
                var msg = messages[i];
                if (msg.Role == AuthorRole.System)
                    tempHistory.AddSystemMessage(msg.Content ?? "");
                else if (msg.Role == AuthorRole.User)
                    tempHistory.AddUserMessage(msg.Content ?? "");
                else if (msg.Role == AuthorRole.Assistant)
                    tempHistory.AddAssistantMessage(msg.Content ?? "");
            }

            // Add the actual RAG prompt for LLM processing
            var ragPrompt = $"""
                Based on the following context from documents, please answer the question. If the answer cannot be found in the context, say "I don't have information about that in the documents."

                Context:
                {context}

                Question: {question}
                """;

            tempHistory.AddUserMessage(ragPrompt);

            // Step 6: Get AI response using the temp history
            var response = await _chatService.GetChatMessageContentAsync(tempHistory);

            // Step 7: Add AI response to the main history
            var answer = response.Content ?? "I couldn't generate an answer.";
            history.AddAssistantMessage(answer);
            
            return answer;
        }
    }
}
