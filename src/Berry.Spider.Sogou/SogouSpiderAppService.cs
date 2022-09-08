using Volo.Abp.Application.Services;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗爬虫
/// </summary>
public class SogouSpiderAppService : ApplicationService, ISogouSpiderAppService
{
    // private IDistributedEventBus DistributedEventBus { get; }
    //
    // public SogouSpiderAppService(IDistributedEventBus eventBus)
    // {
    //     this.DistributedEventBus = eventBus;
    // }
    //
    // /// <summary>
    // /// 向队列推送源数据
    // /// </summary>
    // public Task PushAsync(SogouSpiderPushEto push)
    // {
    //     if (!string.IsNullOrEmpty(push.Keyword))
    //     {
    //         return this.DistributedEventBus.PublishAsync(push);
    //     }
    //
    //     return Task.CompletedTask;
    // }

    // /// <summary>
    // /// 执行获取一级页面数据任务
    // /// </summary>
    // public Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    // {
    //     ISogouSpiderProvider? provider = request.SourceFrom switch
    //     {
    //         SpiderSourceFrom.Sogou_Related_Search => this.LazyServiceProvider.LazyGetRequiredService<SogouSpider4RelatedSearchProvider>(),
    //         _ => throw new SpiderBizException("Not Implemented...")
    //     };
    //
    //     return provider.ExecuteAsync(request);
    // }
    //
    // /// <summary>
    // /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    // /// </summary>
    // public Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    // {
    //     ISogouSpiderProvider? provider = eventData.SourceFrom switch
    //     {
    //         SpiderSourceFrom.Sogou_Related_Search => this.LazyServiceProvider.LazyGetRequiredService<SogouSpider4RelatedSearchProvider>(),
    //         _ => throw new SpiderBizException("Not Implemented...")
    //     };
    //
    //     return provider.HandleEventAsync(eventData);
    // }
}