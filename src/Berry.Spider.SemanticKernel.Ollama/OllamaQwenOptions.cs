namespace Berry.Spider.SemanticKernel.Ollama;

public class OllamaQwenOptions
{
    /// <summary>
    /// 模型ID
    /// </summary>
    public string ModelId { get; set; }

    /// <summary>
    /// 文本嵌入模型ID
    /// </summary>
    public string EmbeddingModelId { get; set; }
}