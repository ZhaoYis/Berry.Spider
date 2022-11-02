using System.Threading.Tasks;
using Berry.Spider.Abstractions;
using Berry.Spider.Sogou;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Consumers;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
public class SogouSpider4RelatedSearchEventHandler : ICapSubscribe
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4RelatedSearchPushEto.RoutingKeyString, Group = SogouSpider4RelatedSearchPushEto.QueueNameString)]
    public async Task HandleEventAsync(SogouSpider4RelatedSearchPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<SogouSpider4RelatedSearchProvider>();

        await provider.ExecuteAsync(new SogouSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(SogouSpider4RelatedSearchPullEto.RoutingKeyString, Group = SogouSpider4RelatedSearchPullEto.QueueNameString)]
    public Task HandleEventAsync(SogouSpider4RelatedSearchPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<SogouSpider4RelatedSearchProvider>();

        return provider.HandleEventAsync(eventData);
    }
}