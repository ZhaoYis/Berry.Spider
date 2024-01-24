using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Consumers;

public class ServLifetimeCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ServLifetimeCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer,
        serviceScopeFactory)
    {
        Timer.Period = 10 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        AppDomain appDomain = AppDomain.CurrentDomain;
        object? applicationProcess = appDomain.GetData(AppGlobalConstants.SPIDER_CLIENT_KEY);
        if (applicationProcess is ApplicationProcessData app)
        {
            IRedisService? redisService = workerContext.ServiceProvider.GetService<IRedisService>();
            if (!string.IsNullOrEmpty(app.ClientId) && redisService is { })
            {
                // 获取当前进程
                Process currentProcess = Process.GetCurrentProcess();
                if (currentProcess.Id == app.Pid)
                {
                    string memoryUsage = $"{currentProcess.WorkingSet64 / 1024 / 1024:#,##0} MB";
                    string cpuUsage = await this.GetCurrentProcessCpuUsageAsync();
                    
                    ApplicationLifetimeData lifetime = new ApplicationLifetimeData
                    {
                        AreYouOk = true,
                        MachineName = Environment.MachineName,
                        ProcessId = app.Pid,
                        ProcessName = appDomain.FriendlyName,
                        MemoryUsage = memoryUsage,
                        CpuUsage = cpuUsage
                    };
                    await redisService.HSetAsync(AppGlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, app.ClientId, JsonSerializer.Serialize(lifetime));

                    Console.WriteLine("PID\tProcess Name\tMemory Usage\tCPU Usage");
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", lifetime.ProcessId, lifetime.ProcessName, lifetime.MemoryUsage, lifetime.CpuUsage);
                }
            }
        }
    }

    private async Task<string> GetCurrentProcessCpuUsageAsync()
    {
        DateTime startTime = DateTime.UtcNow;
        TimeSpan processorTimeBeforeSpin = Process.GetCurrentProcess().TotalProcessorTime;

        //等待500ms
        await Task.Delay(500).ConfigureAwait(false);

        TimeSpan processorTimeAfterSpin = Process.GetCurrentProcess().TotalProcessorTime;
        DateTime endTime = DateTime.UtcNow;

        double timeDiff = (endTime - startTime).TotalMilliseconds;
        double cpuTimeDiff = (processorTimeAfterSpin - processorTimeBeforeSpin).TotalMilliseconds;

        double cpuUsage = cpuTimeDiff / (timeDiff * Environment.ProcessorCount);

        string cpuUsageString = cpuUsage.ToString("P");
        return cpuUsageString;
    }
}