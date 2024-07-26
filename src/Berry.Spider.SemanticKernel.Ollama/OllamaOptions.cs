namespace Berry.Spider.SemanticKernel.Ollama;

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
    public string ServiceAddr { get; set; }
}