using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public interface ISpiderPushToQueueDto : ITraceCode, ISpiderEto
{
    /// <summary>
    /// 关键字
    /// </summary>
    string Keyword { get; set; }

    /// <summary>
    /// 计算当前入队组合唯一标识
    /// </summary>
    /// <returns></returns>
    string GetIdentityId();
}