using Berry.Spider.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Berry.Spider.Biz;

[Route("api/services/serv-machine")]
public class ServMachineController : SpiderControllerBase
{
    private IServMachineAppService ServMachineAppService { get; }

    public ServMachineController(IServMachineAppService servMachineAppService)
    {
        this.ServMachineAppService = servMachineAppService;
    }

    /// <summary>
    /// 根据机器名称获取机器信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("getByMachineName")]
    public Task<ServMachineDto?> GetByMachineNameAsync(string machineName)
    {
        return this.ServMachineAppService.GetByMachineNameAsync(machineName);
    }

    /// <summary>
    /// 根据机器ConnectionId获取机器信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("getByConnectionId"), DisableDataWrapper]
    public Task<ServMachineDto?> GetByConnectionIdAsync(string connectionId)
    {
        return this.ServMachineAppService.GetByConnectionIdAsync(connectionId);
    }

    /// <summary>
    /// 上线
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("online"), DisableDataWrapper]
    public Task<bool> OnlineAsync([FromBody] ServMachineOnlineDto online)
    {
        return this.ServMachineAppService.OnlineAsync(online);
    }

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("offline")]
    public Task<bool> OfflineAsync([FromBody] ServMachineOfflineDto offline)
    {
        return this.ServMachineAppService.OfflineAsync(offline);
    }

    /// <summary>
    /// 获取机器列表
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("getList"), DisableDataWrapper]
    public Task<PagedResultDto<ServMachineDto>> GetListAsync(GetListInput input)
    {
        return this.ServMachineAppService.GetListAsync(input);
    }
}