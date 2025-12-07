using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class ReviewerOutput
{
    /**
     * {
          "overallScore": 85,
          "accuracy": {
            "score": 38,
            "issues": ["问题描述1", "问题描述2"]
          },
          "logic": {
            "score": 25,
            "issues": ["问题描述1"]
          },
          "originality": {
            "score": 18,
            "issues": []
          },
          "formatting": {
            "score": 9,
            "issues": ["问题描述1"]
          },
          "recommendation": "通过",
          "summary": "总体评价和具体修改建议"
        }
     */
    /// <summary>
    /// 总评分
    /// </summary>
    [JsonPropertyName("overallScore")]
    public int OverallScore { get; set; }

    /// <summary>
    /// 准确性评分
    /// </summary>
    [JsonPropertyName("accuracy")]
    public AccuracyScore Accuracy { get; set; } = new();

    /// <summary>
    /// 逻辑评分
    /// </summary>
    [JsonPropertyName("logic")]
    public LogicScore Logic { get; set; } = new();

    /// <summary>
    /// 原创性评分
    /// </summary>
    [JsonPropertyName("originality")]
    public OriginalityScore Originality { get; set; } = new();

    /// <summary>
    /// 格式化评分
    /// </summary>
    [JsonPropertyName("formatting")]
    public FormattingScore Formatting { get; set; } = new();

    /// <summary>
    /// 推荐
    /// </summary>
    [JsonPropertyName("recommendation")]
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>
    /// 总结
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class AccuracyScore
{
    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("issues")] public List<string> Issues { get; set; } = [];
}

public class LogicScore
{
    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("issues")] public List<string> Issues { get; set; } = [];
}

public class OriginalityScore
{
    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("issues")] public List<string> Issues { get; set; } = [];
}

public class FormattingScore
{
    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("issues")] public List<string> Issues { get; set; } = [];
}