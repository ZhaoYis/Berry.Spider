using Berry.Spider.Biz;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Application;

public class ServMachineAppService : CrudAppService<
    ServMachineInfo,
    ServMachineDto,
    int,
    GetListInput,
    ServMachineCreateInput,
    ServMachineUpdateInput>, IServMachineAppService
{
    public ServMachineAppService(IRepository<ServMachineInfo, int> repository) : base(repository)
    {
    }

    /// <summary>
    /// 根据机器名称获取机器信息
    /// </summary>
    /// <returns></returns>
    public async Task<ServMachineDto?> GetByMachineNameAsync(string machineName)
    {
        ServMachineInfo? servMachineInfo = await this.Repository.FindAsync(m => m.MachineName == machineName);
        if (servMachineInfo is not null)
        {
            ServMachineDto dto = this.ObjectMapper.Map<ServMachineInfo, ServMachineDto>(servMachineInfo);
            return dto;
        }

        return default;
    }

    /// <summary>
    /// 根据机器ConnectionId获取机器信息
    /// </summary>
    /// <returns></returns>
    public async Task<ServMachineDto?> GetByConnectionIdAsync(string connectionId)
    {
        ServMachineInfo? servMachineInfo = await this.Repository.FindAsync(m => m.ConnectionId == connectionId);
        if (servMachineInfo is not null)
        {
            ServMachineDto dto = this.ObjectMapper.Map<ServMachineInfo, ServMachineDto>(servMachineInfo);
            return dto;
        }

        return default;
    }

    /// <summary>
    /// 上线
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OnlineAsync(ServMachineOnlineDto online)
    {
        ServMachineInfo servMachineInfo = this.ObjectMapper.Map<ServMachineOnlineDto, ServMachineInfo>(online);
        servMachineInfo.BizNo = this.GuidGenerator.Create().ToString("N")[..10];
        servMachineInfo.Status = MachineStatus.Online;
        servMachineInfo.LastOnlineTime = this.Clock.Now;

        var res = await this.Repository.InsertAsync(servMachineInfo);
        return res.Id > 0;
    }

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OfflineAsync(ServMachineOfflineDto offline)
    {
        ServMachineInfo? servMachineInfo = await this.Repository.FindAsync(m => m.ConnectionId == offline.ConnectionId);
        if (servMachineInfo is not null)
        {
            servMachineInfo.Status = MachineStatus.Offline;
            servMachineInfo.IsDeleted = true;

            await this.Repository.UpdateAsync(servMachineInfo);
        }

        return true;
    }
}