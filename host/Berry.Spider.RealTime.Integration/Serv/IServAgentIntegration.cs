using Refit;

namespace Berry.Spider.RealTime;

public interface IServAgentIntegration
{
    /// <summary>
    /// 上线
    /// </summary>
    /// <returns></returns>
    [Post("/api/services/serv-machine/online")]
    Task<ApiResp<bool>> OnlineAsync([Body] ServMachineOnlineDto online);

    /// <summary>
    /// 下线
    /// </summary>
    /// <returns></returns>
    [Post("/api/services/serv-machine/offline")]
    Task<ApiResp<bool>> OfflineAsync([Body] ServMachineOfflineDto offline);
}