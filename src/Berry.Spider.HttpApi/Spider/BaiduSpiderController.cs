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
    /// 执行爬虫任务
    /// </summary>
    [HttpPost, Route("execute")]
    public async Task ExecuteAsync(BaiduSpiderRequest request)
    {
        //TODO：交给后台任务执行，执行完成后发出通知即可
        await this.BaiduSpiderAppService.ExecuteAsync(request);
    }
}