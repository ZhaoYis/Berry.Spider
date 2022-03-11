using Berry.Spider.Baidu;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 百度爬虫服务
/// </summary>
[Route("api/services/spider/bai-du")]
public class BaiduSpiderController : SpiderControllerBase
{
    private IBaiduSpiderAppService BaiduSpiderAppService { get; }

    public BaiduSpiderController(IBaiduSpiderAppService service)
    {
        this.BaiduSpiderAppService = service;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public async Task PushAsync(BaiduSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        await this.BaiduSpiderAppService.PushAsync(push);
    }
}