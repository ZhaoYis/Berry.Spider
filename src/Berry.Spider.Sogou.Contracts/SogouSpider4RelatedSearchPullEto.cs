using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
[EventName(EventNameString)]
public class SogouSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public const string EventNameString = "Berry.Sogou.RelatedSearch.Pull";

    public SogouSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Sogou_Related_Search)
    {
    }
}