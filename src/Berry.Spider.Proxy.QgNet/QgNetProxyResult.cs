using System.Text.Json.Serialization;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyResult
{
    // {
    //     "IP": "117.57.96.98",
    //     "port": "29220",
    //     "deadline": "2022-09-11 15:13:01",
    //     "host": "117.57.96.98:29220",
    //     "region": "安徽省淮北市电信"
    // }

    /// <summary>
    /// 节点ID
    /// </summary>
    [JsonPropertyName("IP")]
    public string IP { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [JsonPropertyName("port")]
    public string Port { get; set; }

    /// <summary>
    /// 失效日期
    /// </summary>
    [JsonPropertyName("deadline")]
    public DateTime Deadline { get; set; }

    /// <summary>
    /// IP:Port
    /// </summary>
    [JsonPropertyName("host")]
    public string Host { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; }
}