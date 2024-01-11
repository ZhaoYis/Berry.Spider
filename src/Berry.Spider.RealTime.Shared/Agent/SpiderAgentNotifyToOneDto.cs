using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("SendToOneAsync")]
public class SpiderAgentNotifyToOneDto : NotifyMessageBase<string>
{
    /// <summary>
    /// 链接ID
    /// </summary>
    public string ConnectionId { get; set; }
}