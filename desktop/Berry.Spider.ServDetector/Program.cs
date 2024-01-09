using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Berry.Spider.ServDetector.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Berry.Spider.ServDetector;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs-.txt", rollingInterval: RollingInterval.Hour))
#if DEBUG
            .WriteTo.Async(c => c.Console())
#endif
            .CreateLogger();

        try
        {
            Log.Information("Starting console host.");

            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.RegisterCommand(opt =>
                    {
                        opt.Commands.Add(RealTimeMessageCode.SYSTEM_MESSAGE.GetName(), typeof(SystemMessageCommand));
                        opt.Commands.Add(RealTimeMessageCode.CONNECTION_SUCCESSFUL.GetName(), typeof(ConnectionSuccessfulCommand));
                        opt.Commands.Add(RealTimeMessageCode.NOTIFY_AGENT_TO_START_DEPLOYING_APP.GetName(), typeof(NotifyAgentToStartDeployingAppCommand));
                    });

                    services.AddHostedService<ServDetectorHostedService>();
                    services.AddHostedService<ServAgentHostedService>();
                })
                .UseAgileConfig()
                .UseSerilog()
                .RunConsoleAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}