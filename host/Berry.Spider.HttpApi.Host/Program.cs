using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Berry.Spider.Consumers;
using Berry.Spider.Core;
using Berry.Spider.HttpApi.Host;
using Serilog;
using Serilog.Events;

namespace Berry.Spider;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "DEV";
        var configuration = GetConfiguration(args);
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.With<ThreadIdEnricher>()
            //Debug
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Debug-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} DEBUG [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Info
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Info-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} INFO [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Warn
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Warn-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} WARN [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Error
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Error-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ERROR [http-api-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            Log.Information(JsonSerializer.Serialize(configuration.AsEnumerable().AsDictionary(), new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                //机密配置文件
                .AddAppSettingsSecretsJson()
                .UseAgileConfig($"appsettings.{env}.json")
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