using Berry.Spider.Domain.Shared;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[EventName("Berry.TouTiao.Question")]
public class TouTiaoSpider4QuestionEto : SpiderPullBaseEto
{
    public TouTiaoSpider4QuestionEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }

    /// <summary>
    /// 保存这次记录最终的标题
    /// </summary>
    public string Title { get; set; }
}