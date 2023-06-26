using Berry.Spider.Core;

namespace Berry.Spider;

public interface ISpiderPubAndRecAppService
{
    /// <summary>
    /// 清理待执行任务数据
    /// </summary>
    /// <returns></returns>
    Task ClearTodoTaskAsync(SpiderSourceFrom from);
}