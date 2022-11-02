using System.Threading.Tasks;
using Berry.Spider.Abstractions;
using Berry.Spider.Baidu;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Consumers;

/// <summary>
/// 百度：相关搜索
/// </summary>
public class BaiduSpider4RelatedSearchEventHandler : ICapSubscribe
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    [CapSubscribe(BaiduSpider4RelatedSearchPushEto.RoutingKeyString, Group = BaiduSpider4RelatedSearchPushEto.QueueNameString)]
    public async Task HandleEventAsync(BaiduSpider4RelatedSearchPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>();

        await provider.ExecuteAsync(new BaiduSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    [CapSubscribe(BaiduSpider4RelatedSearchPullEto.RoutingKeyString, Group = BaiduSpider4RelatedSearchPullEto.QueueNameString)]
    public Task HandleEventAsync(BaiduSpider4RelatedSearchPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>();

        return provider.HandleEventAsync(eventData);
    }
}