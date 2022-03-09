using Berry.Spider.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
[Route("api/services/spider")]
public class SpiderController : SpiderControllerBase
{
    private ISpiderAppService SpiderAppService { get; }

    public SpiderController(ISpiderAppService service)
    {
        this.SpiderAppService = service;
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
}