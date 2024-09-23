using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Services;
using OllamaSharp;
using OllamaSharp.Models;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen;

[Experimental("SKEXP0001")]
public class OllamaQwenTextEmbeddingGenerationService : IEmbeddingGenerationService<string, double>
{
    private readonly OllamaApiClient _ollamaApiClient;

    private OllamaOptions OllamaOptions { get; }
    private OllamaQwenOptions OllamaQwenOptions { get; }

    public OllamaQwenTextEmbeddingGenerationService(IOptionsSnapshot<OllamaOptions> ollamaOptions, IOptionsSnapshot<OllamaQwenOptions> qwenOPtions)
    {
        this.OllamaOptions = ollamaOptions.Value;
        this.OllamaQwenOptions = qwenOPtions.Value;
        this.Attributes = new Dictionary<string, object?>
        {
            [AIServiceExtensions.ModelIdKey] = this.OllamaQwenOptions.EmbeddingModelId ?? OllamaQwenConsts.DEFAULT_TEXT_EMBEDDING_MODEL_ID_KEY,
            [AIServiceExtensions.EndpointKey] = this.OllamaOptions.ServiceAddr,
        };
        _ollamaApiClient = new OllamaApiClient(this.OllamaOptions.ServiceAddr, this.OllamaQwenOptions.EmbeddingModelId ?? OllamaQwenConsts.DEFAULT_TEXT_EMBEDDING_MODEL_ID_KEY);
    }

    /// <summary>
    /// Gets the AI service attributes.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Attributes { get; }

    /// <summary>
    /// 生成文本向量
    /// </summary>
    /// <returns></returns>
    public async Task<IList<ReadOnlyMemory<double>>> GenerateEmbeddingsAsync(IList<string> data, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var result = new List<ReadOnlyMemory<double>>(data.Count);

        foreach (var text in data)
        {
            GenerateEmbeddingRequest embeddingRequest = new GenerateEmbeddingRequest
            {
                Model = this.Attributes[AIServiceExtensions.ModelIdKey]?.ToString() ?? string.Empty,
                Prompt = text
            };
            GenerateEmbeddingResponse embeddingResponse = await _ollamaApiClient.GenerateEmbeddings(embeddingRequest, cancellationToken);
            if (embeddingResponse is { Embedding: { Length: > 0 } })
            {
                var embedding = new ReadOnlyMemory<double>(embeddingResponse.Embedding);
                result.Add(embedding);
            }
        }

        return result;
    }
}