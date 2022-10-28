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
}