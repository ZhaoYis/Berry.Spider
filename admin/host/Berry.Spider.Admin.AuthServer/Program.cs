using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Berry.Spider.Admin;

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
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} DEBUG [http-authserver-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Info
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Info-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} INFO [http-authserver-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Warn
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Warn-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} WARN [http-authserver-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
            //Error
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                .WriteTo.Async(c => c.File(
                    path: "Logs/logs-Error-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ERROR [http-authserver-host] [] [] [] [] [{ThreadId}] --- {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)))
#if DEBUG
            .WriteTo.Async(c => c.Console())
#endif
            .CreateLogger();

        try
        {
            Log.Information("Starting Berry.Spider.Admin.AuthServer host.");
            Log.Information($"CommandLine：{Environment.CommandLine}");

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                Log.Information("{EntryKey}={EntryValue}", entry.Key, entry.Value);
            }

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "DEV";
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAgileConfig($"appsettings.{env}.json")
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<AdminAuthServerModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Berry.Spider.Admin.AuthServer terminated unexpectedly!");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}