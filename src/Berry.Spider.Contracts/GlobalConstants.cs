namespace Berry.Spider;

public static class GlobalConstants
{
    /// <summary>
    /// Parallel最大并行数
    /// </summary>
    public const int ParallelMaxDegreeOfParallelism = 10;

    /// <summary>
    /// 用于判重的Redis的key
    /// </summary>
    public const string SPIDER_KEYWORDS_KEY = "Keywords";
}