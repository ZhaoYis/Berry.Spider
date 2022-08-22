using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
[EventName("Berry.Sogou.RelatedSearch.Pull")]
public class SogouSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public SogouSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Sogou_Related_Search)
    {
    }
}