using Berry.Spider.AI.TextGeneration.Commands;
using Berry.Spider.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Berry.Spider.AI.TextGeneration;

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
                        opt.Commands.Add(nameof(WatcherChangeTypes.Created), typeof(FileOrFolderCreatedCommand));
                        opt.Commands.Add(nameof(WatcherChangeTypes.Deleted), typeof(FileOrFolderDeletedCommand));
                        opt.Commands.Add(nameof(WatcherChangeTypes.Changed), typeof(FileOrFolderChangedCommand));
                        opt.Commands.Add(nameof(WatcherChangeTypes.Renamed), typeof(FileOrFolderRenamedCommand));
                    });

                    services.AddHostedService<AITextGenerationHostedService>();
                    services.AddHostedService<FileWatcherHostedService>();
                })
                //集成AgileConfig
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