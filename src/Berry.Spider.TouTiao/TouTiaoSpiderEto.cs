using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条资讯
/// </summary>
[EventName("Berry.TouTiao.Information")]
public class TouTiaoSpiderEto : SpiderBaseEto
{
    public List<TouTiaoDataItem> Items { get; set; } = new List<TouTiaoDataItem>();
}

public class TouTiaoDataItem
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 跳转地址
    /// </summary>
    public string Href { get; set; }
}