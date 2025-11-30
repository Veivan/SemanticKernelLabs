using OllamaChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;
using OllamaSharp;
using Lab4;

var builder = Kernel.CreateBuilder();

var ollamaChat = new OllamaChatCompletionService(new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"));
builder.Services.AddKeyedSingleton<IChatCompletionService>("ollamaChat", ollamaChat);

//builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

var kernel = builder.Build();

// #### Using Built-in SK Plugins: Loading Functions

#pragma warning disable SKEXP0050
var timePluginFunctions = kernel.ImportPluginFromObject(new TimePlugin(), "time");
var arguments = new KernelArguments() { ["input"] = "100" };
var result = await kernel.InvokeAsync(timePluginFunctions["daysAgo"], arguments);
Console.WriteLine("100 days ago was " + result + Environment.NewLine);
#pragma warning restore SKEXP0050

// #### Loading and calling functions from CustomPlugin

#pragma warning disable SKEXP0050
var customPluginFunctions = kernel.ImportPluginFromObject(new CustomPlugin(kernel));
Console.WriteLine($"{string.Join("\n", customPluginFunctions.Select(_ => $"[{_.Name}] : {_.Description}"))}");
Console.WriteLine(Environment.NewLine);

arguments = new KernelArguments()
{
    { "BooksNumber", "10" },
    { "YearFrom", "1900" },
    { "YearTo", "2000" },
    { "lang", "Russian" },
    { "Input", "Hi, I'm looking for the best historical book suggestions top-{{$BooksNumber}} from {{$YearFrom}} to {{$YearTo}}" }
};

result = await kernel.InvokeAsync(customPluginFunctions["Translate"], arguments);
Console.WriteLine(result + Environment.NewLine);

arguments.Remove("lang");
arguments.Add("lang", "Turkche");
result = await kernel.InvokeAsync(customPluginFunctions["Translate"], arguments);
Console.WriteLine(result + Environment.NewLine);

#pragma warning restore SKEXP0050

