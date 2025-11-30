using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

namespace OllamaChatCompletion
{
    internal class OllamaChatCompletionService : IChatCompletionService
    {
        private readonly OllamaApiClient _ollamaClient;

        public OllamaChatCompletionService(OllamaApiClient ollamaClient)
        {
            _ollamaClient = ollamaClient;
       }

        public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var chat = new Chat(_ollamaClient);

            // iterate though chatHistory Messages
            foreach (var message in chatHistory)
            {
                if (message.Role == AuthorRole.System)
                {
                    chat.SendAsync(message.Content);
                    continue;
                }
            }

            var lastMessage = chatHistory.LastOrDefault();

            string question = lastMessage.Content;
            var chatResponse = "";

            await foreach (var answerToken in chat.SendAsync(question))
            {
                chatResponse += answerToken;
            }

            chatHistory.AddAssistantMessage(chatResponse);

            var chatHistoryNew = new ChatHistory();
            chatHistoryNew.AddAssistantMessage(chatResponse);

            return chatHistoryNew;
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
