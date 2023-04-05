using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IUserAgentProvider : ISingletonDependency
{
    /// <summary>
    /// 随机从User-Agent池中获取一个User-Agent
    /// </summary>
    /// <returns></returns>
    Task<string> GetOnesAsync();
}