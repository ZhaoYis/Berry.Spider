using System.Net;

namespace Berry.Spider.Proxy.QgNet;

internal static class CacheKeyHelper
{
    private static readonly string CurrentHostName = Dns.GetHostName();

    public static readonly string QgNetProxyResultCacheKey = $"QgNetProxyResultCacheKey:{CurrentHostName}";

    public static readonly string QgNetProxyQuotaResultCacheKey = "QgNetProxyQuotaResultCacheKey";
}