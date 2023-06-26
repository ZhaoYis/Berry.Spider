using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

[Route("api/services/spiderPubAndRec")]
public class SpiderPubAndRecController : SpiderControllerBase
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