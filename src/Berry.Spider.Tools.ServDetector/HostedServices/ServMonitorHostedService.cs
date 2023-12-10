using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.ServDetector;

public class ServMonitorHostedService : IHostedService
{
    private readonly ILogger<ServMonitorHostedService> _logger;
    private readonly HubConnection _connection;

    public ServMonitorHostedService(ILogger<ServMonitorHostedService> logger)
    {
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44382/signalr-hubs/spider/monitor-notify")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ReceiveSystemMessageDto>(typeof(ReceiveSystemMessageDto).GetMethodName(), async msg =>
        {
            string? connectionId = _connection.ConnectionId;
            if (msg.Code == ReceiveMessageCode.CONNECTION_SUCCESSFUL)
            {
                if (msg.Data == connectionId)
                {
                    MonitorAgentClientInfoDto agentClientInfo = new MonitorAgentClientInfoDto
                    {
                        Code = NotifyMessageCode.PUSH_MONITOR_CLIENT_INFO,
                        Data = new MonitorClientInfo
                        {
                            MachineName = DnsHelper.GetHostName(),
                            ConnectionId = _connection.ConnectionId
                        }
                    };
                    await _connection.SendAsync(typeof(MonitorAgentClientInfoDto).GetMethodName(), agentClientInfo);
                }
            }
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                await _connection.StartAsync(cancellationToken);
                break;
            }
            catch (Exception e)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _connection.DisposeAsync();
    }
}