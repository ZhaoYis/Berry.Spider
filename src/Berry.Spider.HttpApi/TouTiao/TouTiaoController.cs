using Berry.Spider.TouTiao.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条
/// </summary>
[Route("api/services/spider/tou-tiao")]
public class TouTiaoController : SpiderController
{
    private ITouTiaoSpiderService TiaoSpiderService { get; }

    public TouTiaoController(ITouTiaoSpiderService service)
    {
        this.TiaoSpiderService = service;
    }

    [HttpPost, Route("execute")]
    public async Task ExecuteAsync(TouTiaoSpiderRequest request)
    {
        await this.TiaoSpiderService.ExecuteAsync(request);
    }
}