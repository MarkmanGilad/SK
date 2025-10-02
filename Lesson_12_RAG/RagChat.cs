using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_12_RAG
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
            // Step 1: Search for relevant documents
            var relevantDocs = await _chromaClient.Search(question, maxResults: 3);

            // Step 2: Create context from found documents
            var context = string.Join("\n\n", relevantDocs);

            // Step 3: Add the question with context to chat history
            var ragPrompt = $"""
                Use the following context to answer the question. If the answer cannot be found in the context, say "I don't have information about that in the documents."

                Context:
                {context}

                Question: {question}
                """;

            history.AddUserMessage(ragPrompt);

            // Step 4: Get AI response
            var response = await _chatService.GetChatMessageContentAsync(history);

            // Step 5: Add AI response to history
            history.AddAssistantMessage(response.Content);
            return response.Content;
        }
    }
}