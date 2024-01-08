using Berry.Spider.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider.Biz;

[Area(AppGlobalConstants.ModelName)]
[Route("api/services/app-info")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class SpiderAppInfoController : SpiderControllerBase, ISpiderAppInfoService
{
    private readonly ISpiderAppInfoService _spiderAppInfoService;

    public SpiderAppInfoController(ISpiderAppInfoService spiderAppInfoService)
    {
        _spiderAppInfoService = spiderAppInfoService;
    }

    /// <summary>
    /// 获取最近几个版本应用信息
    /// </summary>
    /// <param name="top">最近N个</param>
    /// <returns></returns>
    [HttpGet, Route("getAppList"), DisableDataWrapper]
    public Task<List<SpiderAppInfoDto>> GetSpiderAppListAsync(int top = 3)
    {
        return _spiderAppInfoService.GetSpiderAppListAsync(top);
    }

    /// <summary>
    /// 获取某个应用的详细信息
    /// </summary>
    /// <param name="bizNo">业务编码</param>
    /// <returns></returns>
    [HttpGet, Route("getAppInfo"), DisableDataWrapper]
    public Task<SpiderAppInfoDto> GetSpiderAppInfoAsync(string bizNo)
    {
        return _spiderAppInfoService.GetSpiderAppInfoAsync(bizNo);
    }
}