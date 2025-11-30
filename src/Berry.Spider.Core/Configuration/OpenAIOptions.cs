namespace Berry.Spider.Core;

public class OpenAIOptions
{
    /// <summary>
    /// 是否启用OpenAI
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// ApiKey
    /// </summary>
    public string ApiKey { get; set; }

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