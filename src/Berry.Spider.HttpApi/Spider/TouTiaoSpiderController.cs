using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Route("api/services/spider/tou-tiao")]
public class TouTiaoSpiderController : SpiderControllerBase
{
    private IDistributedEventBus DistributedEventBus { get; }


    public TouTiaoSpiderController(IDistributedEventBus eventBus)
    {
        this.DistributedEventBus = eventBus;
    }

    // /// <summary>
    // /// 执行爬虫任务
    // /// </summary>
    // [HttpPost, Route("execute")]
    // public async Task ExecuteAsync(TouTiaoSpiderRequest request)
    // {
    //     //直接发布事件到MQ，交由Berry.Spider.Consumers消费
    //     
    //     await this.TiaoSpiderService.ExecuteAsync(request);
    // }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public async Task PushAsync(TouTiaoSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        await this.DistributedEventBus.PublishAsync(push);
    }
}