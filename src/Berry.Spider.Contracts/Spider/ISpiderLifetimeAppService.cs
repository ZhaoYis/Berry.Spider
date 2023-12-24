using Volo.Abp.Application.Services;

namespace Berry.Spider;

public interface ISpiderLifetimeAppService : IApplicationService
{
    /// <summary>
    /// 获取爬虫服务状态
    /// </summary>
    /// <returns></returns>
    Task<List<ApplicationLifetimeDto>> GetSpiderStatusAsync();
}