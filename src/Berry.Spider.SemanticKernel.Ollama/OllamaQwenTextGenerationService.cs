using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using OllamaSharp;

namespace Berry.Spider.SemanticKernel.Ollama;

public class OllamaQwenTextGenerationService : ITextGenerationService
{
    private readonly OllamaApiClient _ollamaApiClient;

    private OllamaOptions OllamaOptions { get; }
    private OllamaQwenOptions OllamaQwenOptions { get; }

    public OllamaQwenTextGenerationService(IOptions<OllamaOptions> ollamaOptions, IOptions<OllamaQwenOptions> ollamaQwenOptions)
    {
        this.OllamaOptions = ollamaOptions.Value;
        this.OllamaQwenOptions = ollamaQwenOptions.Value;
        this.Attributes = new Dictionary<string, object?>
        {
            [AIServiceExtensions.ModelIdKey] = this.OllamaQwenOptions.ModelId,
            [AIServiceExtensions.EndpointKey] = this.OllamaOptions.ServiceAddr,
        };
        _ollamaApiClient = new OllamaApiClient(this.OllamaOptions.ServiceAddr, this.OllamaQwenOptions.ModelId);
    }

    /// <summary>
    /// Gets the AI service attributes.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Attributes { get; }

    /// <summary>
    /// 同步内容生成
    /// </summary>
    /// <returns></returns>
    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        List<TextContent> textContents = new List<TextContent>();
        ConversationContext? context = null;
        var completionResponse = _ollamaApiClient.Generate(prompt, context, cancellationToken); //.GetCompletion(prompt, context, cancellationToken);
        await foreach (var res in completionResponse)
        {
            TextContent textContent = new TextContent(res?.Response);
            textContents.Add(textContent);
        }

        return textContents;
    }

    /// <summary>
    /// 流式内容生成
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ConversationContext? context = null;
        var completionResponseStream = _ollamaApiClient.Generate(prompt, context, cancellationToken); //.StreamCompletion(prompt, context, cancellationToken);
        await foreach (var cps in completionResponseStream)
        {
            yield return new StreamingTextContent(cps?.Response);
        }
    }
}