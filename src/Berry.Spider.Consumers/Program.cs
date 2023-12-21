using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Berry.Spider.Core;
using Microsoft.Extensions.Configuration;

namespace Berry.Spider.Consumers;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var configuration = GetConfiguration(args);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("DotNetCore.CAP", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With<ThreadIdEnricher>()
            //Debug
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Debug-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} DEBUG [berry-spider-consumers] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Info
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Info-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} INFO [berry-spider-consumers] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Warn
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Warn-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} WARN [berry-spider-consumers] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Error
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Error-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ERROR [berry-spider-consumers] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            .WriteTo.Async(c => c.Console(
                restrictedToMinimumLevel: LogEventLevel.Information
            ))
            .CreateLogger();

        try
        {
            Log.Information("Starting console host.");
            Log.Information(JsonSerializer.Serialize(configuration.AsEnumerable().AsDictionary(), new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            await Host.CreateDefaultBuilder(args).ConfigureServices(services => { services.AddHostedService<SpiderConsumersHostedService>(); })
                //机密配置文件
                .AddAppSettingsSecretsJson()
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

    private static IConfiguration GetConfiguration(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (args is { Length: > 0 })
        {
            string pattern = "^--environment=(.+)$";
            var envArg = args.FirstOrDefault(arg => Regex.IsMatch(arg, pattern));
            if (!string.IsNullOrWhiteSpace(envArg))
            {
                var envArgGroups = Regex.Match(envArg, pattern).Groups;
                if (envArgGroups.Count == 2)
                {
                    env = envArgGroups[1].Value;
                }
            }
        }

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }
}