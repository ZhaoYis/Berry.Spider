using Berry.Spider.Core.Exceptions;
using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度爬虫
/// </summary>
public class BaiduSpiderAppService : ApplicationService, IBaiduSpiderAppService
{
    private IDistributedEventBus DistributedEventBus { get; }

    public BaiduSpiderAppService(IDistributedEventBus eventBus)
    {
        this.DistributedEventBus = eventBus;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    public Task PushAsync(BaiduSpiderPushEto push)
    {
        if (!string.IsNullOrEmpty(push.Keyword))
        {
            return this.DistributedEventBus.PublishAsync(push);
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        IBaiduSpiderProvider? provider = request.SourceFrom switch
        {
            SpiderSourceFrom.Baidu_RelatedSearch => this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        return provider.ExecuteAsync(request);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        IBaiduSpiderProvider? provider = eventData.SourceFrom switch
        {
            SpiderSourceFrom.Baidu_RelatedSearch => this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        return provider.HandleEventAsync(eventData);
    }
}