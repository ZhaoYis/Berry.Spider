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
    /// 上线
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OnlineAsync(ServMachineOnlineDto online)
    {
        ServMachineDto? servMachineDto = await this.GetByMachineNameAsync(online.MachineName);
        if (servMachineDto is not null)
        {
            ServMachineInfo servMachineInfo = this.ObjectMapper.Map<ServMachineDto, ServMachineInfo>(servMachineDto);
            servMachineInfo.Status = MachineStatus.Online;
            servMachineInfo.LastOnlineTime = this.Clock.Now;

            await this.Repository.UpdateAsync(servMachineInfo);
        }
        else
        {
            ServMachineInfo servMachineInfo = this.ObjectMapper.Map<ServMachineOnlineDto, ServMachineInfo>(online);
            servMachineInfo.BizNo = this.GuidGenerator.Create().ToString("N");
            servMachineInfo.Status = MachineStatus.Online;
            servMachineInfo.LastOnlineTime = this.Clock.Now;

            await this.Repository.InsertAsync(servMachineInfo);
        }

        return true;
    }

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OfflineAsync(ServMachineOfflineDto offline)
    {
        ServMachineDto? servMachineDto = await this.GetByMachineNameAsync(offline.MachineName);
        if (servMachineDto is not null)
        {
            ServMachineInfo servMachineInfo = this.ObjectMapper.Map<ServMachineDto, ServMachineInfo>(servMachineDto);
            servMachineInfo.Status = MachineStatus.Offline;

            await this.Repository.UpdateAsync(servMachineInfo);
        }

        return true;
    }
}