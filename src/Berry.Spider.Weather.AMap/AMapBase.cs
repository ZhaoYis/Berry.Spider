using System.Text.Json.Serialization;

namespace Berry.Spider.Weather.AMap;

public class AMapBase
{
    /// <summary>
    /// 返回状态。值为0或1。1：成功；0：失败
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// 返回的状态信息
    /// </summary>
    [JsonPropertyName("info")]
    public string Info { get; set; }

    /// <summary>
    /// 返回状态说明,10000代表正确
    /// </summary>
    [JsonPropertyName("infocode")]
    public string InfoCode { get; set; }

    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool IsSuccessful => this.Status.Equals("1") && this.InfoCode.Equals("10000");
}