using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

public class NotifyMessageBase<T>
{
    /// <summary>
    /// 通知类型编码
    /// </summary>
    public RealTimeMessageCode Code { get; set; }

    /// <summary>
    /// 业务数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 附带消息
    /// </summary>
    public string Message { get; set; }
}