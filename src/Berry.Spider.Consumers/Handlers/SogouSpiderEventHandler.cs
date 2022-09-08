using Berry.Spider.Abstractions;
using Berry.Spider.Sogou;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 搜狗分布式事件处理器
/// </summary>
public class SogouSpiderEventHandler :
    IDistributedEventHandler<SogouSpider4RelatedSearchPushEto>,
    IDistributedEventHandler<SogouSpider4RelatedSearchPullEto>, ITransientDependency
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

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

    public Task HandleEventAsync(SogouSpider4RelatedSearchPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<SogouSpider4RelatedSearchProvider>();

        return provider.HandleEventAsync(eventData);
    }
}