namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyQuotaResult
{
    /**
     *  -1 未知错误
        -10 参数不合法!
        -11 请求过于频繁!
        -100 计划不存在或已过期!
     */
    public int Code { get; set; }

    /// <summary>
    /// 总数量
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 可用数量
    /// </summary>
    public int Available { get; set; }

    /// <summary>
    /// 已使用数量
    /// </summary>
    public int Used { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess => this.Code == 0;
}