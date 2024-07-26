using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen2;

public class OllamaQwen2TextGenerationService : ITextGenerationService
{
    public IReadOnlyDictionary<string, object?> Attributes { get; } = new Dictionary<string, object?>
    {
        [AIServiceExtensions.ModelIdKey] = "qwen2:7b"
    };

    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = new CancellationToken())
    {
        return null;
    }

    public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = new CancellationToken())
    {
        return null;
    }
}