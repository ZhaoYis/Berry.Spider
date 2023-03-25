namespace Berry.Spider.Weixin.Work;

public class MarkdownMessageDto : IWeixinRobotMessage
{
    public MarkdownMessageDto(string content)
    {
        markdown = new MarkdownContent
        {
            content = content
        };
    }

    public string msgtype => "markdown";

    public MarkdownContent markdown { get; }
}

public class MarkdownContent
{
    public string content { get; set; }
}