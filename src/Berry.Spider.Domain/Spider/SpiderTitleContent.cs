using Berry.Spider.Domain.Shared;

namespace Berry.Spider.Domain;

/// <summary>
/// 只保存标题和链接
/// </summary>
public class SpiderTitleContent : EntityBase
{
    /// <summary>
    /// 已采集
    /// </summary>
    public int Collected { get; set; } = 1;

    /// <summary>
    /// 已发布
    /// </summary>
    public int Published { get; set; } = 0;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 内容（当前标题对应的跳转地址）
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 出处
    /// </summary>
    public string From { get; set; }

    public SpiderTitleContent()
    {
    }

    public SpiderTitleContent(
        string title,
        string content,
        SpiderSourceFrom from)
    {
        this.Title = title;
        this.Content = content;
        this.Author = from.ToString();
        this.From = from.ToString();
        this.Time = DateTimeOffset.Now.DateTime;
    }
}