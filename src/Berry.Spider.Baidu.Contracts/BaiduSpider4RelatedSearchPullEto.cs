using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关搜索
/// </summary>
[EventName(RoutingKeyString)]
public class BaiduSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.Baidu.RelatedSearch.Pull";
    public const string QueueNameString = "Baidu.RelatedSearch.Pull";

    public BaiduSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Baidu_Related_Search)
    {
    }
}