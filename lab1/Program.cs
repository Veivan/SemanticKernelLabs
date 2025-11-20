using Lab1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OllamaSharp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // ---------- 1) Build Kernel with an IChatClient (Extensions.AI) ----------
        // Ollama:
        var builder = Kernel.CreateBuilder();
        builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"));
 
        // Build the kernel
        Kernel kernel = builder.Build();

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        // ---------- 3) Import a tiny SK plugin that performs search ----------
        kernel.ImportPluginFromType<LookForPlugin>("LookForPlugin");

        KernelArguments kernelArgs = new(openAIPromptExecutionSettings)
        {
            { "input", "I want to find top-10 books about world history" }
        };

        string skPrompt = @"ChatBot: How can I help you?
User: {{$input}}

---------------------------------------------
If you need to search for books, respond with: CALL PerformSearch: <search terms>
Return data requested by user: ";

        var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, openAIPromptExecutionSettings);
        var result = await chatFunction.InvokeAsync(kernel, kernelArgs);

        string llmResponse = result.ToString();

        if (llmResponse.StartsWith("CALL PerformSearch:"))
        {
            string searchQuery = llmResponse.Replace("CALL PerformSearch:", "").Trim();
            kernelArgs = new(openAIPromptExecutionSettings)
            {
                { "input", llmResponse },
                { "category", "history" }
            };
            var plug = new LookForPlugin();
            var results = plug.PerformSearch(kernelArgs);
            Console.WriteLine(results);
        }
        else
            Console.WriteLine(result.ToString());
    }
}