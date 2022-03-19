using Berry.Spider.Core.Exceptions;
using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条爬虫
/// </summary>
public class TouTiaoSpiderAppService : ApplicationService, ITouTiaoSpiderAppService
{
    private IDistributedEventBus DistributedEventBus { get; }
    
    public TouTiaoSpiderAppService(IDistributedEventBus eventBus)
    {
        this.DistributedEventBus = eventBus;
    }
    
    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    public async Task PushAsync(TouTiaoSpiderPushEto push)
    {
        if (!string.IsNullOrEmpty(push.Keyword))
        {
            await this.DistributedEventBus.PublishAsync(push);
        }
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        ITouTiaoSpiderProvider? provider = request.SourceFrom switch
        {
            SpiderSourceFrom.TouTiao_Question => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>(),
            SpiderSourceFrom.TouTiao_Information => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        await provider.ExecuteAsync(request);
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public async Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        ITouTiaoSpiderProvider? provider = eventData.SourceFrom switch
        {
            SpiderSourceFrom.TouTiao_Question => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>(),
            SpiderSourceFrom.TouTiao_Information => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationProvider>(),
            _ => throw new SpiderBizException("Not Implemented...")
        };

        await provider.HandleEventAsync(eventData);
    }
}