using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[EventName(EventNameString)]
public class TouTiaoSpider4QuestionPullEto : SpiderPullBaseEto
{
    public const string EventNameString = "Berry.TouTiao.Question.Pull";
    
    public TouTiaoSpider4QuestionPullEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }
}