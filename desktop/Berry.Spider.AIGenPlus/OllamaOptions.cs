namespace Berry.Spider.AIGenPlus;

/// <summary>
/// Ollama相关配置选项
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// 是否启用Ollama服务
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 服务地址
    /// </summary>
    public required string ServiceAddr { get; set; }

    /// <summary>
    /// 默认模型ID
    /// </summary>
    public required string ModelId { get; set; }

    /// <summary>
    /// 默认文本嵌入模型ID
    /// </summary>
    public required string EmbeddingModelId { get; set; }
}