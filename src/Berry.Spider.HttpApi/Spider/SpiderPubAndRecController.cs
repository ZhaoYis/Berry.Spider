using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider;

[Area(AppGlobalConstants.ModelName)]
[Route("api/services/spiderPubAndRec")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class SpiderPubAndRecController : SpiderControllerBase, ISpiderPubAndRecAppService
{
    private readonly ISpiderPubAndRecAppService _spiderPubAndRecAppService;

    public SpiderPubAndRecController(ISpiderPubAndRecAppService spiderPubAndRecAppService)
    {
        _spiderPubAndRecAppService = spiderPubAndRecAppService;
    }

    /// <summary>
    /// 清理待执行任务数据
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("clearTodoTask")]
    public async Task ClearTodoTaskAsync(SpiderSourceFrom from)
    {
        await _spiderPubAndRecAppService.ClearTodoTaskAsync(from);
    }
}