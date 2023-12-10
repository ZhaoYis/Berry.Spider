using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("SendToAllAsync")]
public class SpiderMonitorNotifyDto : NotifyMessageBase<string>
{
}