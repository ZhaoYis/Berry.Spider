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
    public async Task PushAsync(BaiduSpiderPushEto push)
    {
        if (push.Keywords.Any())
        {
            await this.DistributedEventBus.PublishAsync(push);
        }
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        IBaiduSpiderProvider? provider = request.SourceFrom switch
        {
            SpiderSourceFrom.Baidu_RelatedSearch => this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        await provider.ExecuteAsync(request);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public async Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        IBaiduSpiderProvider? provider = eventData.SourceFrom switch
        {
            SpiderSourceFrom.Baidu_RelatedSearch => this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        await provider.HandleEventAsync(eventData);
    }
}