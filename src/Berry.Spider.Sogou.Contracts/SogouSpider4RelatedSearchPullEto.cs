using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
[EventName(RoutingKeyString)]
public class SogouSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.Sogou.RelatedSearch.Pull";
    public const string QueueNameString = "Sogou.RelatedSearch.Pull";

    public SogouSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Sogou_Related_Search)
    {
    }
}