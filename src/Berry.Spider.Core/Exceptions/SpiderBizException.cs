namespace Berry.Spider.Core;

/// <summary>
/// 爬虫业务异常
/// </summary>
public class SpiderBizException : Exception
{
    public SpiderBizException(string msg) : base(message: msg)
    {
    }
}