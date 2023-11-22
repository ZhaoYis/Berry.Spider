using System.Text.Json.Serialization;
using Volo.Abp.Caching;

namespace Berry.Spider.Proxy.QgNet;

[CacheName("QgNetProxyResult")]
public class QgNetProxyResult
{
    /**
     * {
        "code": "SUCCESS",
        "data": [{
        "proxy_ip": "123.54.55.24",
        "server": "123.54.55.24:59419",
        "area": "河南省商丘市",
        "isp": "电信",
        "deadline": "2023-02-25 15:38:36"
        }],
        "request_id": "83158ebe-be6c-40f7-a158-688741083edc"
        }
     */
    /// <summary>
    /// 节点ID
    /// </summary>
    [JsonPropertyName("proxy_ip")]
    public string ProxyIP { get; set; }

    /// <summary>
    /// 失效日期
    /// </summary>
    [JsonPropertyName("deadline")]
    public string Deadline { get; set; }

    /// <summary>
    /// IP:Port
    /// </summary>
    [JsonPropertyName("server")]
    public string Server { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    [JsonPropertyName("area")]
    public string Area { get; set; }

    /// <summary>
    /// 运营商
    /// </summary>
    [JsonPropertyName("isp")]
    public string ISP { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsInvalid => (DateTime.Parse(this.Deadline) - DateTime.Now).TotalSeconds > 0;
}