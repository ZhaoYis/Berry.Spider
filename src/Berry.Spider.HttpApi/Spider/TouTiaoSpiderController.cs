using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Route("api/services/spider/tou-tiao")]
public class TouTiaoSpiderController : SpiderControllerBase
{
    private ITouTiaoSpiderAppService TiaoSpiderService { get; }

    public TouTiaoSpiderController(ITouTiaoSpiderAppService service)
    {
        this.TiaoSpiderService = service;
    }

    /// <summary>
    /// 执行爬虫任务
    /// </summary>
    [HttpPost, Route("execute")]
    public async Task ExecuteAsync(TouTiaoSpiderRequest request)
    {
        //TODO：交给后台任务执行，执行完成后发出通知即可
        await this.TiaoSpiderService.ExecuteAsync(request);
    }
}