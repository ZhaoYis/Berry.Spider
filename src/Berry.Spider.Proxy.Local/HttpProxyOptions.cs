namespace Berry.Spider.Proxy;

public class HttpProxyOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 代理池API地址
    /// </summary>
    public string ProxyPoolApiHost { get; set; }
}