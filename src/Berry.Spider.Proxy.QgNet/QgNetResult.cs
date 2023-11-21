using System.Text.Json.Serialization;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetResult<T> where T : class, new()
{
    /// <summary>
    /// 请求状态码。
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }

    /// <summary>
    /// IP资源列表。
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; }

    /// <summary>
    /// 唯一请求ID，每次请求都会返回。定位问题时需要提供该次请求的 request_id。
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess => this.Code == "SUCCESS";
}