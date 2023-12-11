using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.ServDetector;

public class ServMonitorAgentHostedService : IHostedService
{
    private readonly ILogger<ServMonitorAgentHostedService> _logger;
    private readonly HubConnection _connection;

    public ServMonitorAgentHostedService(ILogger<ServMonitorAgentHostedService> logger)
    {
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44382/signalr-hubs/spider/monitor-notify")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ReceiveSystemMessageDto>(typeof(ReceiveSystemMessageDto).GetMethodName(), async msg =>
        {
            if (msg.Code == RealTimeMessageCode.CONNECTION_SUCCESSFUL)
            {
                MonitorAgentClientInfoDto agentClientInfo = new MonitorAgentClientInfoDto
                {
                    Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL,
                    Data = new MonitorClientInfo
                    {
                        MachineName = DnsHelper.GetHostName(),
                        ConnectionId = _connection.ConnectionId
                    }
                };
                await _connection.SendAsync(typeof(MonitorAgentClientInfoDto).GetMethodName(), agentClientInfo);
            }
            else if (msg.Code == RealTimeMessageCode.SYSTEM_MESSAGE)
            {
                Console.WriteLine(msg.Message);
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