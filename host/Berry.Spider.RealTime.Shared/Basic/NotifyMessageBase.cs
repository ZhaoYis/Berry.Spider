namespace Berry.Spider.RealTime;

public class NotifyMessageBase<T> where T : class
{
    /// <summary>
    /// 通知类型编码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 业务数据
    /// </summary>
    public T Data { get; set; }
}