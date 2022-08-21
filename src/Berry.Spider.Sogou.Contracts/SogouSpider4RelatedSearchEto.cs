using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
[EventName("Berry.Sogou.RelatedSearch.Pull")]
public class SogouSpider4RelatedSearchEto : SpiderPullBaseEto
{
    public SogouSpider4RelatedSearchEto() : base(SpiderSourceFrom.Sogou_Related_Search)
    {
    }
}