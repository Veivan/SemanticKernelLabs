using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var huggingFaceSettings = config.GetRequiredSection("HuggingFaceSettings");
string apiKey = Environment.GetEnvironmentVariable("HUGGING_FACE_API_KEY");
                 
var builder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    builder.Services.AddHuggingFaceTextGeneration(
        model: huggingFaceSettings["Model"],
        apiKey: apiKey);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    Console.WriteLine("Using HuggingFaceSettings model");

var kernel = builder.Build();

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

KernelArguments kernelArgs = new(openAIPromptExecutionSettings)
{
    { "input", "I want to find top-10 books about Artificial Intelligence" }
};

string skPrompt = @"ChatBot: How can I help you?
User: {{$input}}

---------------------------------------------

Return data requested by user: ";

var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, openAIPromptExecutionSettings);
var result = await chatFunction.InvokeAsync(kernel, kernelArgs);

Console.WriteLine(result.ToString());

