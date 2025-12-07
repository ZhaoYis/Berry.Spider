using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class SummarizeOutput
{
    /// <summary>
    /// 主题分析
    /// </summary>
    [JsonPropertyName("topic_analysis")]
    public string TopicAnalysis { get; set; } = string.Empty;

    /// <summary>
    /// 关键要点
    /// </summary>
    [JsonPropertyName("key_points")]
    public List<KeyPoint> KeyPoints { get; set; } = new();

    /// <summary>
    /// 技术细节
    /// </summary>
    [JsonPropertyName("technical_details")]
    public List<TechnicalDetail> TechnicalDetails { get; set; } = new();

    /// <summary>
    /// 代码示例
    /// </summary>
    [JsonPropertyName("code_examples")]
    public List<CodeExample> CodeExamples { get; set; } = new();

    /// <summary>
    /// 引用
    /// </summary>
    [JsonPropertyName("references")]
    public List<string> References { get; set; } = new();
}

public class KeyPoint
{
    /// <summary>
    /// 重要性
    /// </summary>
    [JsonPropertyName("importance")]
    public int Importance { get; set; } // 1-3, 3最高

    /// <summary>
    /// 内容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class TechnicalDetail
{
    /// <summary>
    /// 标题
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class CodeExample
{
    /// <summary>
    /// 语言
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// 代码
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}