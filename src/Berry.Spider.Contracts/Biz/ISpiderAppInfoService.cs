using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Biz;

public interface ISpiderAppInfoService : IApplicationService, ITransientDependency
{
    /// <summary>
    /// 获取最近几个版本应用信息
    /// </summary>
    /// <param name="top">最近N个</param>
    /// <returns></returns>
    Task<List<SpiderAppInfoDto>> GetSpiderAppListAsync(int top = 3);

    /// <summary>
    /// 获取某个应用的详细信息
    /// </summary>
    /// <param name="bizNo">业务编码</param>
    /// <returns></returns>
    Task<SpiderAppInfoDto> GetSpiderAppInfoAsync(string bizNo);
}