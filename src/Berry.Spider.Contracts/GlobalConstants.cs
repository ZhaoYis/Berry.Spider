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
    public const string SPIDER_KEYWORDS_KEY = "Keywords";
}