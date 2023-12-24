using Berry.Spider.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Berry.Spider.Biz;

[Area(GlobalConstants.ModelName)]
[Route("api/services/serv-machine")]
[RemoteService(Name = GlobalConstants.RemoteServiceName)]
public class ServMachineController : SpiderControllerBase, IServMachineAppService
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

    [HttpGet, Route("get")]
    public Task<ServMachineDto> GetAsync(int id)
    {
        throw new NotImplementedException();
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

    [HttpPost, Route("create")]
    public Task<ServMachineDto> CreateAsync(ServMachineCreateInput input)
    {
        return this.ServMachineAppService.CreateAsync(input);
    }

    [HttpPut, Route("update")]
    public Task<ServMachineDto> UpdateAsync(int id, ServMachineUpdateInput input)
    {
        return this.ServMachineAppService.UpdateAsync(id, input);
    }

    [HttpDelete, Route("delete")]
    public Task DeleteAsync(int id)
    {
        return this.ServMachineAppService.DeleteAsync(id);
    }
}