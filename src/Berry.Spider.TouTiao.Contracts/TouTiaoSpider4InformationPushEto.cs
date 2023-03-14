using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Information)]
public class TouTiaoSpider4InformationPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Information.Push";
    public const string QueueNameString = "TouTiao.Information.Push";
}