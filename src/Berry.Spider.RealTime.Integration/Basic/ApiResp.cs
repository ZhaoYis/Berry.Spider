using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

public class ApiResp<T>
{
    /// <summary>
    /// 返回code
    /// </summary>
    public ApiErrorCodes Code { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// 返回消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 响应类型。'success' | 'error' | 'warning'
    /// </summary>
    public string Type { get; set; }

    public bool IsSuccessful => this.Type == "success" && this.Code == ApiErrorCodes.OK;
}