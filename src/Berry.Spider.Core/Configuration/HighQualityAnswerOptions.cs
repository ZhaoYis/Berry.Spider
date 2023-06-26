namespace Berry.Spider.Contracts;

public class HighQualityAnswerOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 最短长度
    /// </summary>
    public int MinLength { get; set; }

    /// <summary>
    /// 最大长度
    /// </summary>
    public int MaxLength { get; set; }

    /// <summary>
    /// 最多记录数
    /// </summary>
    public int MaxRecordCount { get; set; } = 10;

    /// <summary>
    /// 落库最小记录数
    /// </summary>
    public int MinSaveRecordCount { get; set; }
}