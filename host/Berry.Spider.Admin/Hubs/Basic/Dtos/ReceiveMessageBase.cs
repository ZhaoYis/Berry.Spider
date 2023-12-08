namespace Berry.Spider.Admin;

public class ReceiveMessageBase<T> where T : class
{
    /// <summary>
    /// 消息类型编码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 业务数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 附带消息
    /// </summary>
    public string Message { get; set; }
}