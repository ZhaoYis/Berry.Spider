using Berry.Spider.Core;
using Volo.Abp.Application.Services;

namespace Berry.Spider;

public interface ISpiderPubAndRecAppService : IApplicationService
{
    /// <summary>
    /// 清理待执行任务数据
    /// </summary>
    /// <returns></returns>
    Task ClearTodoTaskAsync(SpiderSourceFrom from);
}