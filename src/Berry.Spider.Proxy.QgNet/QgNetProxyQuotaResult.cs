using System.Text.Json.Serialization;
using Volo.Abp.Caching;

namespace Berry.Spider.Proxy.QgNet;

[CacheName("QgNetProxyQuotaResult")]
public class QgNetProxyQuotaResult
{
    /// <summary>
    /// 余额
    /// </summary>
    [JsonPropertyName("balance")]
    public int Balance { get; set; }
}