using System.ComponentModel.DataAnnotations;

namespace Berry.Spider.Contracts;

/// <summary>
/// 关键字校验选项
/// </summary>
public class KeywordCheckOptions
{
    /// <summary>
    /// Bloom过滤器
    /// </summary>
    public bool BloomCheck { get; set; } = false;

    /// <summary>
    /// Redis
    /// </summary>
    public bool RedisCheck { get; set; } = false;

    /// <summary>
    /// 关键字验证范围是否只是当前分类下唯一
    /// </summary>
    public bool OnlyCurrentCategory { get; set; } = false;

    /// <summary>
    /// 是否启用相似度检查
    /// </summary>
    public bool IsEnableSimilarityCheck { get; set; } = false;

    /// <summary>
    /// 最小相似度（原关键字和采集到的标题之间的相似度），取值范围为[0,100]
    /// </summary>
    [Range(0, 100)]
    public int MinSimilarity { get; set; }
}