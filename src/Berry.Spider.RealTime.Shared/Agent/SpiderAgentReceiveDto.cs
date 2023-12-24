using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("ReceiveMessageAsync")]
public class SpiderAgentReceiveDto : ReceiveMessageBase<string>
{
}