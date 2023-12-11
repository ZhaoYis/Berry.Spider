using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Biz;

public interface IServMachineAppService : ICrudAppService<
    ServMachineDto,
    int,
    GetListInput,
    ServMachineCreateInput,
    ServMachineUpdateInput>, ITransientDependency
{
    /// <summary>
    /// 根据机器名称获取机器信息
    /// </summary>
    /// <returns></returns>
    Task<ServMachineDto?> GetByMachineNameAsync(string machineName);

    /// <summary>
    /// 上线
    /// </summary>
    /// <returns></returns>
    Task<bool> OnlineAsync(ServMachineOnlineDto online);

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    Task<bool> OfflineAsync(ServMachineOfflineDto offline);
}