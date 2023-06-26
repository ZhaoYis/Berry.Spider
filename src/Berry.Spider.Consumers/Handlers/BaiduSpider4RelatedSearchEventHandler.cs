using Berry.Spider.Baidu;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 百度：相关搜索
/// </summary>
public class BaiduSpider4RelatedSearchEventHandler : ICapSubscribe
{
    private BaiduSpider4RelatedSearchProvider Provider { get; }

    public BaiduSpider4RelatedSearchEventHandler(BaiduSpider4RelatedSearchProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(BaiduSpider4RelatedSearchPushEto.RoutingKeyString, Group = BaiduSpider4RelatedSearchPushEto.QueueNameString)]
    public async Task HandleEventAsync(BaiduSpider4RelatedSearchPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(BaiduSpider4RelatedSearchPullEto.RoutingKeyString, Group = BaiduSpider4RelatedSearchPullEto.QueueNameString)]
    public async Task HandleEventAsync(BaiduSpider4RelatedSearchPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}