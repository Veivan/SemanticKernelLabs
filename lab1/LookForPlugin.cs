using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Lab1
{
    internal class LookForPlugin
    {
        [KernelFunction("PerformSearch"), Description("Method to search books in library")]
        public string PerformSearch(KernelArguments? args)
        {
            if (args == null)
            {
                return "No Books identified";
            }

            bool hasCategory = args.TryGetValue("category", out var category);
            bool hasAuthor = args.TryGetValue("author", out var author);

            if (hasCategory && hasAuthor)
            {
                return "7 books identified"; // Assuming both conditions together identify 7 books
            }
            else if (hasCategory)
            {
                return "5 books identified";
            }
            else if (hasAuthor)
            {
                return "2 books identified";
            }
            else
            {
                return "No Books identified";
            }
        }
    }
}
