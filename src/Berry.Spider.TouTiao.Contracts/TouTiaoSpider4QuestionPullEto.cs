using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[EventName(RoutingKeyString)]
public class TouTiaoSpider4QuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Question.Pull";
    public const string QueueNameString = "TouTiao.Question.Pull";
    
    public TouTiaoSpider4QuestionPullEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }
}