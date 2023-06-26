namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 代理池API地址
    /// </summary>
    public string ProxyPoolApiHost { get; set; }

    /// <summary>
    /// 申请的Key值
    /// </summary>
    public string AuthKey { get; set; }
}