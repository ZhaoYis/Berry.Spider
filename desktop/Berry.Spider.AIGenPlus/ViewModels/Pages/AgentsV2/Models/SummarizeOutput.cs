using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class SummarizeOutput
{
    [JsonPropertyName("topic_analysis")] public string TopicAnalysis { get; set; } = string.Empty;

    [JsonPropertyName("key_points")] public List<KeyPoint> KeyPoints { get; set; } = new();

    [JsonPropertyName("technical_details")]
    public List<TechnicalDetail> TechnicalDetails { get; set; } = new();

    [JsonPropertyName("code_examples")] public List<CodeExample> CodeExamples { get; set; } = new();

    [JsonPropertyName("references")] public List<string> References { get; set; } = new();
}

public class KeyPoint
{
    [JsonPropertyName("importance")] public int Importance { get; set; } // 1-3, 3最高

    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}

public class TechnicalDetail
{
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
}

public class CodeExample
{
    [JsonPropertyName("language")] public string Language { get; set; } = string.Empty;

    [JsonPropertyName("code")] public string Code { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
}