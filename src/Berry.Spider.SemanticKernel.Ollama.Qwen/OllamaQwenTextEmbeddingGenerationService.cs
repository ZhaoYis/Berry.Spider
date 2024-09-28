using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Services;
using OllamaSharp;
using OllamaSharp.Models;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

public class OllamaQwenTextEmbeddingGenerationService : ITextEmbeddingGenerationService
{
    private readonly OllamaApiClient _ollamaApiClient;

    private OllamaOptions OllamaOptions { get; }
    private OllamaQwenOptions OllamaQwenOptions { get; }

    public OllamaQwenTextEmbeddingGenerationService(IOptions<OllamaOptions> ollamaOptions, IOptions<OllamaQwenOptions> qwenOPtions)
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
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, Kernel? kernel = null, CancellationToken cancellationToken = default)
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

        return this.ConvertToFloatList(result);
    }

    private List<ReadOnlyMemory<float>> ConvertToFloatList(List<ReadOnlyMemory<double>> doubleList)
    {
        List<ReadOnlyMemory<float>> floatList = new List<ReadOnlyMemory<float>>();

        foreach (var memory in doubleList)
        {
            // 获取当前 ReadOnlyMemory<double> 的长度，以元素数量计
            int doubleCount = memory.Length;
            // 计算需要的 float 数组的大小
            int floatCount = doubleCount * sizeof(double) / sizeof(float);
            // 创建新的 float 数组
            var floatArray = new float[floatCount];

            // 将 double 值转换为 float 并复制到新数组
            int fi = 0;
            foreach (var item in memory.Span)
            {
                floatArray[fi++] = (float)item;
            }

            // 将 float 数组转换为 ReadOnlyMemory<float> 并添加到列表中
            floatList.Add(floatArray);
        }

        return floatList;
    }
}