using Berry.Spider.Domain.Shared;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：资讯
/// </summary>
[EventName("Berry.TouTiao.Information")]
public class TouTiaoSpider4InformationEto : SpiderBaseEto
{
    public TouTiaoSpider4InformationEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }

    /// <summary>
    /// 保存这次记录最终的标题
    /// </summary>
    public string Title { get; set; }
}