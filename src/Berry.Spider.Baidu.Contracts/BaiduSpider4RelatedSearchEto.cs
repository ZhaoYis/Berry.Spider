using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关搜索
/// </summary>
[EventName("Berry.Baidu.RelatedSearch.Pull")]
public class BaiduSpider4RelatedSearchEto : SpiderPullBaseEto
{
    public BaiduSpider4RelatedSearchEto() : base(SpiderSourceFrom.Baidu_Related_Search)
    {
    }
}