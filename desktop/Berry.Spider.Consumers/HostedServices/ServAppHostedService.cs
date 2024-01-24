using System;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.Core;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Consumers;

public class ServAppHostedService : IHostedService
{
    private readonly ILogger<ServAppHostedService> _logger;
    private readonly HubConnection _connection;
    private RealTimeOptions? RealTimeOptions { get; }

    public ServAppHostedService(ILogger<ServAppHostedService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        RealTimeOptions = configuration.GetSection(nameof(RealTimeOptions)).Get<RealTimeOptions>();

        if (RealTimeOptions is not null)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(RealTimeOptions.AppEndpointUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.On<ReceiveSystemMessageDto>(typeof(ReceiveSystemMessageDto).GetMethodName(), async msg =>
            {
                if (msg.Code == RealTimeMessageCode.CONNECTION_SUCCESSFUL)
                {
                    AppClientInfoDto appClientInfo = new AppClientInfoDto
                    {
                        Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL,
                        Data = new AppClientInfo
                        {
                            MachineName = DnsHelper.GetHostName(),
                            MachineCode = $"{MachineGroupCode.App.GetName()}_{Guid.NewGuid().ToString("N")[..10]}",
                            MachineIpAddr = DnsHelper.GetIpV4s(),
                            MachineMacAddr = DnsHelper.GetMacAddress(),
                            ConnectionId = _connection.ConnectionId
                        }
                    };
                    await _connection.SendToAsync<AppClientInfoDto>(appClientInfo);
                }
                else if (msg.Code == RealTimeMessageCode.SYSTEM_MESSAGE)
                {
                    Console.WriteLine(msg.Message);
                }
            });
        }
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
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _connection.DisposeAsync();
    }
}