using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Consumers;

public class ServLifetimeCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ServLifetimeCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
    {
        Timer.Period = 5 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        AppDomain appDomain = AppDomain.CurrentDomain;
        object? applicationProcess = appDomain.GetData(GlobalConstants.SPIDER_CLIENT_KEY);
        if (applicationProcess is ApplicationProcess app)
        {
            IRedisService? redisService = workerContext.ServiceProvider.GetService<IRedisService>();
            if (!string.IsNullOrEmpty(app.ClientId) && redisService is { })
            {
                Process? currentProcess = Process.GetProcesses().FirstOrDefault(p => p.Id == app.Pid);
                if (currentProcess is { })
                {
                    string memoryUsage = string.Format("{0:#,##0} MB", currentProcess.WorkingSet64 / 1024 / 1024);
                    string cpuUsage = "";
                    if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        cpuUsage = "None";
                    }
                    else
                    {
                        cpuUsage = string.Format("{0:0.00}%", currentProcess.TotalProcessorTime / Process.GetCurrentProcess().TotalProcessorTime * 100);
                    }

                    ApplicationLifetime lifetime = new ApplicationLifetime
                    {
                        AreYouOk = true,
                        MachineName = Environment.MachineName,
                        ProcessId = app.Pid,
                        ProcessName = appDomain.FriendlyName,
                        MemoryUsage = memoryUsage,
                        CpuUsage = cpuUsage
                    };
                    await redisService.HSetAsync(GlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, app.ClientId, JsonSerializer.Serialize(lifetime));

                    Console.WriteLine("PID\tProcess Name\tMemory Usage\tCPU Usage");
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", lifetime.ProcessId, lifetime.ProcessName, lifetime.MemoryUsage, lifetime.CpuUsage);
                }
            }
        }
    }
}