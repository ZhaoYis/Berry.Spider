using Berry.Spider.Baidu;
using System.Threading.Tasks;
using Berry.Spider.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 百度分布式事件处理器
/// </summary>
public class BaiduSpiderEventHandler :
    IDistributedEventHandler<BaiduSpider4RelatedSearchPushEto>,
    IDistributedEventHandler<BaiduSpider4RelatedSearchPullEto>, ITransientDependency
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

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

    public Task HandleEventAsync(BaiduSpider4RelatedSearchPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>();

        return provider.HandleEventAsync(eventData);
    }
}