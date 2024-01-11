using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Berry.Spider.RealTime;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.ServDetector;

public class ServAgentHostedService : IHostedService
{
    private readonly ILogger<ServAgentHostedService> _logger;
    private readonly HubConnection _connection;
    private IServiceScopeFactory ServiceScopeFactory { get; }
    private RealTimeOptions? RealTimeOptions { get; }

    public ServAgentHostedService(ILogger<ServAgentHostedService> logger,
        IServiceScopeFactory factory, IConfiguration configuration)
    {
        _logger = logger;
        ServiceScopeFactory = factory;
        RealTimeOptions = configuration.GetSection(nameof(RealTimeOptions)).Get<RealTimeOptions>();

        if (RealTimeOptions is not null)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(RealTimeOptions.AgentEndpointUrl)
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
                            MachineCode = $"{MachineGroupCode.Agent.GetName()}_{Guid.NewGuid().ToString("N")[..10]}",
                            MachineIpAddr = DnsHelper.GetIpV4s(),
                            MachineMacAddr = DnsHelper.GetMacAddress(),
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

            _connection.On<SpiderAgentReceiveDto>(typeof(SpiderAgentReceiveDto).GetMethodName(), async msg =>
            {
                try
                {
                    using var scope = ServiceScopeFactory.CreateScope();
                    var commandSelector = scope.ServiceProvider.GetRequiredService<ICommandSelector>();
                    CommandLineArgs commandLineArgs = new CommandLineArgs
                    {
                        Command = msg.Code.GetName(),
                        Body = msg.Data
                    };
                    var commandType = commandSelector.Select(commandLineArgs);

                    var command = (IFixedCommand)scope.ServiceProvider.GetRequiredService(commandType);
                    await command.ExecuteAsync(commandLineArgs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _connection.DisposeAsync();
    }
}