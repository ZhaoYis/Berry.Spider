namespace Berry.Spider;

public static class GlobalConstants
{
    public static int ParallelMaxDegreeOfParallelism
    {
        get
        {
            //逻辑处理器个数
            return Environment.ProcessorCount / 2;
        }
    }

    /// <summary>
    /// 用于判重的Redis的key
    /// </summary>
    public const string SPIDER_KEYWORDS_KEY = "spider-keywords";

    /// <summary>
    /// 用于判重的Redis的key
    /// </summary>
    public const string SPIDER_KEYWORDS_KEY_PUSH = "spider-keywords-push";
    
    /// <summary>
    /// 用于判重的Redis的key
    /// </summary>
    public const string SPIDER_KEYWORDS_KEY_PULL = "spider-keywords-pull";
    
    /// <summary>
    /// 启动程序基本信息key
    /// </summary>
    public const string SPIDER_CLIENT_KEY = "spider-client";

    /// <summary>
    /// 应用存活状态信息key
    /// </summary>
    public const string SPIDER_APPLICATION_LIFETIME_KEY = "spider-client-lifetime";
}