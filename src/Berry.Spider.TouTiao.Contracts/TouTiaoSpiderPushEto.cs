using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName("Berry.TouTiao.Source.Push")]
public class TouTiaoSpiderPushEto : SpiderPushBaseEto
{
}