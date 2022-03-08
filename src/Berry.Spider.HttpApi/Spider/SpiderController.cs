using Berry.Spider.Contracts;
using Berry.Spider.TouTiao;
using Berry.Spider.TouTiao.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
[Route("api/services/spider")]
public class SpiderController : SpiderControllerBase
{
    private ITouTiaoSpiderAppService TiaoSpiderService { get; }
    private ISpiderAppService SpiderAppService { get; }

    public SpiderController(ITouTiaoSpiderAppService service, ISpiderAppService spiderAppService)
    {
        this.TiaoSpiderService = service;
        this.SpiderAppService = spiderAppService;
    }

    /// <summary>
    /// 获取爬虫列表
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("get-list")]
    public async Task<List<SpiderDto>> GetListAsync(GetListInput input)
    {
        return await this.SpiderAppService.GetListAsync(input);
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