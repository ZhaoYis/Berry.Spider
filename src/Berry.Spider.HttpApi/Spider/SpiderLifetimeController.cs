using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider;

/// <summary>
/// 爬虫状态服务
/// </summary>
[Area(AppGlobalConstants.ModelName)]
[Route("api/services/spider-lifetime")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class SpiderLifetimeController : SpiderControllerBase, ISpiderLifetimeAppService
{
    private readonly ISpiderLifetimeAppService _spiderLifetimeAppService;

    public SpiderLifetimeController(ISpiderLifetimeAppService spiderLifetimeAppService)
    {
        _spiderLifetimeAppService = spiderLifetimeAppService;
    }

    /// <summary>
    /// 获取爬虫服务状态
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("get-status")]
    public Task<List<ApplicationLifetimeDto>> GetSpiderStatusAsync()
    {
        return _spiderLifetimeAppService.GetSpiderStatusAsync();
    }
}