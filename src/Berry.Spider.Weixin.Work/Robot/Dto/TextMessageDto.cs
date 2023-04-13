namespace Berry.Spider.Weixin.Work;

public class TextMessageDto : IWeixinRobotMessage
{
    public TextMessageDto(string content)
    {
        text = new TextContent
        {
            content = content
        };
    }

    public string msgtype => "text";

    public TextContent text { get; }
}

public class TextContent
{
    public string content { get; set; }

    public List<string> mentioned_list { get; set; } = new List<string> { "@all" };
}