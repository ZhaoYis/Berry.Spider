using Refit;

namespace Berry.Spider.RealTime;

public interface IServAgentIntegration
{
    [Post("/api/services/serv-machine/online")]
    Task<bool> OnlineAsync(ServMachineOnlineDto online);

    [Post("/api/services/serv-machine/offline")]
    Task<bool> OfflineAsync(ServMachineOfflineDto offline);
}