using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.ServDetector;

public class ServAgentHostedService : IHostedService
{
    private readonly ILogger<ServAgentHostedService> _logger;
    private readonly HubConnection _connection;

    public ServAgentHostedService(ILogger<ServAgentHostedService> logger)
    {
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44382/signalr-hubs/spider/agent-notify")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ReceiveSystemMessageDto>(typeof(ReceiveSystemMessageDto).GetMethodName(), async msg =>
        {
            if (msg.Code == RealTimeMessageCode.CONNECTION_SUCCESSFUL)
            {
                AgentClientInfoDto agentClientInfo = new AgentClientInfoDto
                {
                    Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL,
                    Data = new AgentClientInfo
                    {
                        MachineName = DnsHelper.GetHostName(),
                        ConnectionId = _connection.ConnectionId
                    }
                };
                await _connection.SendToAsync<AgentClientInfoDto>(agentClientInfo);
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