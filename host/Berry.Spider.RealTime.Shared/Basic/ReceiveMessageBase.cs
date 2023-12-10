using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

public class ReceiveMessageBase<T>
{
    /// <summary>
    /// 消息类型编码
    /// </summary>
    public ReceiveMessageCode Code { get; set; }

    /// <summary>
    /// 业务数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 附带消息
    /// </summary>
    public string Message { get; set; }
}