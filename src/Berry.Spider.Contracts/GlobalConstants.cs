namespace Berry.Spider;

public static class GlobalConstants
{
    /// <summary>
    /// 落库每条记录最小内容数量
    /// </summary>
    public const int DefaultMinRecords = 30;

    /// <summary>
    /// 用于判重的Redis的key
    /// </summary>
    public const string SPIDER_KEYWORDS_KEY = "Keywords";
}