using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using OllamaSharp;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen;

public class OllamaQwenTextGenerationService : ITextGenerationService
{
    private readonly OllamaApiClient _ollamaApiClient;

    private OllamaOptions OllamaOptions { get; }
    private OllamaQwenOptions OllamaQwenOptions { get; }

    public OllamaQwenTextGenerationService(IOptionsSnapshot<OllamaOptions> ollamaOptions, IOptionsSnapshot<OllamaQwenOptions> qwenOPtions)
    {
        this.OllamaOptions = ollamaOptions.Value;
        this.OllamaQwenOptions = qwenOPtions.Value;
        this.Attributes = new Dictionary<string, object?>
        {
            [AIServiceExtensions.ModelIdKey] = this.OllamaQwenOptions.ModelId ?? OllamaQwenConsts.DEFAULT_MODEL_ID_KEY,
            [AIServiceExtensions.EndpointKey] = this.OllamaOptions.ServiceAddr,
        };
        _ollamaApiClient = new OllamaApiClient(this.OllamaOptions.ServiceAddr, this.OllamaQwenOptions.ModelId ?? OllamaQwenConsts.DEFAULT_MODEL_ID_KEY);
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
        ConversationContext? context = null;
        var completionResponse = await _ollamaApiClient.GetCompletion(prompt, context, cancellationToken);
        TextContent textContent = new TextContent(completionResponse.Response);
        return new List<TextContent> { textContent };
    }

    /// <summary>
    /// 流式内容生成
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ConversationContext? context = null;
        var completionResponseStream = _ollamaApiClient.StreamCompletion(prompt, context, cancellationToken);
        await foreach (var cps in completionResponseStream)
        {
            yield return new StreamingTextContent(cps?.Response);
        }
    }
}