using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("ReceiveMessageAsync")]
public class SpiderMonitorReceiveDto : ReceiveMessageBase<string>
{
}