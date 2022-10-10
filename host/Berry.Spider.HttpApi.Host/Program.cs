using Berry.Spider.Consumers;
using Berry.Spider.HttpApi.Host;
using Serilog;
using Serilog.Events;

namespace Berry.Spider;

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
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With<ThreadIdEnricher>()
            //Debug
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Debug-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} DEBUG [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Info
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Info-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} INFO [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Warn
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Warn-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} WARN [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Error
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Error-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} ERROR [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
#if DEBUG
            .WriteTo.Async(c => c.Console())
#endif
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                //机密配置文件
                .AddAppSettingsSecretsJson()
                //集成AgileConfig
                .UseAgileConfig()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<SpiderHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}