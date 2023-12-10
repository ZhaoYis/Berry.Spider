using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("ReceiveSystemMessageAsync")]
public class ReceiveSystemMessageDto : ReceiveMessageBase<string>
{
}