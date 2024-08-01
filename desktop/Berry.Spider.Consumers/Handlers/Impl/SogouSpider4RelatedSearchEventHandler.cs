using Berry.Spider.Sogou;
using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Berry.Spider.Consumers;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
public sealed class SogouSpider4RelatedSearchEventHandler(SogouSpider4RelatedSearchProvider provider) : ISogouSpider4RelatedSearchEventHandler, ICapSubscribe
{
    private SogouSpider4RelatedSearchProvider Provider { get; } = provider;

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4RelatedSearchPushEto.RoutingKeyString, Group = SogouSpider4RelatedSearchPushEto.QueueNameString)]
    public async Task HandleEventAsync(SogouSpider4RelatedSearchPushEto eventData)
    {
        await this.Provider.HandlePushEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4RelatedSearchPullEto.RoutingKeyString, Group = SogouSpider4RelatedSearchPullEto.QueueNameString)]
    public async Task HandleEventAsync(SogouSpider4RelatedSearchPullEto eventData)
    {
        await this.Provider.HandlePullEventAsync(eventData);
    }
}