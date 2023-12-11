using Microsoft.AspNetCore.Mvc;

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
    /// 上线
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("online")]
    public Task<bool> OnlineAsync(ServMachineOnlineDto online)
    {
        return this.ServMachineAppService.OnlineAsync(online);
    }

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("offline")]
    public Task<bool> OfflineAsync(ServMachineOfflineDto offline)
    {
        return this.ServMachineAppService.OfflineAsync(offline);
    }
}