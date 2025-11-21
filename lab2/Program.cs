using Lab2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OllamaSharp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // ---------- 1) Build Kernel with an OllamaChatCompletionService ----------
        // Ollama:
        var builder = Kernel.CreateBuilder();

        var ollamaChat = new OllamaChatCompletionService(new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"));
        builder.Services.AddKeyedSingleton<IChatCompletionService>("ollamaChat", ollamaChat);

        Kernel kernel = builder.Build();

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory("You are a librarian, expert about books.");
        chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");

        Func<string, Task> Chat = async (string input) =>
        {
            chatHistory.AddUserMessage(input);

            // Get the response from the AI
            var answer = await chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            // Show the response
            Console.WriteLine("Assistant > " + answer);
        };

        // first call
        Console.WriteLine("User > " + chatHistory[1]);
        await Chat(chatHistory[1].ToString());

        // Initiate a back-and-forth chat
        string? userInput;
        do
        {
            // Collect user input
            Console.Write("User > ");
            userInput = Console.ReadLine();

            await Chat(userInput);
        } while (userInput is not null);
    }
}