using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Route("api/services/spider/tou-tiao")]
public class TouTiaoSpiderController : SpiderControllerBase
{
    private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }

    public TouTiaoSpiderController(ITouTiaoSpiderAppService service)
    {
        this.TouTiaoSpiderAppService = service;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public async Task PushAsync([FromBody] TouTiaoSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        await this.TouTiaoSpiderAppService.PushAsync(push);
    }
}