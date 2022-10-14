using Berry.Spider.Abstractions;
using Berry.Spider.TouTiao;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Consumers;

/// <summary>
/// 今日头条分布式事件处理器
/// </summary>
public class TouTiaoSpiderEventHandler :
    IDistributedEventHandler<TouTiaoSpider4QuestionPushEto>,
    IDistributedEventHandler<TouTiaoSpider4InformationPushEto>,
    IDistributedEventHandler<TouTiaoSpider4InformationCompositionPushEto>,
    IDistributedEventHandler<TouTiaoSpider4QuestionPullEto>,
    IDistributedEventHandler<TouTiaoSpider4InformationPullEto>,
    IDistributedEventHandler<TouTiaoSpider4InformationCompositionPullEto>,
    ITransientDependency
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <param name="eventData"></param>
    public async Task HandleEventAsync(TouTiaoSpider4QuestionPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>();

        await provider.ExecuteAsync(new TouTiaoSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <param name="eventData">Event data</param>
    public Task HandleEventAsync(TouTiaoSpider4InformationPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationProvider>();

        return provider.ExecuteAsync(new TouTiaoSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <param name="eventData">Event data</param>
    public Task HandleEventAsync(TouTiaoSpider4InformationCompositionPushEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationCompositionProvider>();

        return provider.ExecuteAsync(new TouTiaoSpiderRequest
        {
            SourceFrom = eventData.SourceFrom,
            Keyword = eventData.Keyword
        });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public Task HandleEventAsync(TouTiaoSpider4QuestionPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>();

        return provider.HandleEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public Task HandleEventAsync(TouTiaoSpider4InformationPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationProvider>();

        return provider.HandleEventAsync(eventData);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <param name="eventData">Event data</param>
    public Task HandleEventAsync(TouTiaoSpider4InformationCompositionPullEto eventData)
    {
        ISpiderProvider provider =
            this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationCompositionProvider>();

        return provider.HandleEventAsync(eventData);
    }
}