namespace Berry.Spider.Weixin.Work;

public class MarkdownMessageDto : IWeixinRobotMessage
{
    public MarkdownMessageDto(string content)
    {
        //超过4096个字符会报错
        if (content.Length > 4096)
            content = content.Substring(0, 4096);

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