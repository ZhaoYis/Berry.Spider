using Volo.Abp.DependencyInjection;

namespace Berry.Spider;

public interface ISpiderLifetimeAppService : ITransientDependency
{
    /// <summary>
    /// 获取爬虫服务状态
    /// </summary>
    /// <returns></returns>
    Task<List<ApplicationLifetimeDto>> GetSpiderStatusAsync();
}