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
        Timer.Period = 5 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        AppDomain appDomain = AppDomain.CurrentDomain;
        object? applicationProcess = appDomain.GetData(GlobalConstants.SPIDER_CLIENT_KEY);
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
                    string cpuUsage = this.GetCurrentProcessCpuUsage(currentProcess);

                    ApplicationLifetimeData lifetime = new ApplicationLifetimeData
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

    private string GetCurrentProcessCpuUsage(Process currentProcess)
    {
        // 获取上次CPU时间
        TimeSpan lastCpuTime = currentProcess.TotalProcessorTime;
        // 获取当前时间
        DateTime lastUpdateTime = DateTime.Now;
        // 等待一段时间，例如100毫秒
        Thread.Sleep(500);
        // 获取当前时间
        DateTime currentTime = DateTime.Now;
        // 获取当前CPU时间
        TimeSpan currentCpuTime = currentProcess.TotalProcessorTime;
        // 计算时间间隔内的CPU时间差
        TimeSpan cpuTimeDiff = currentCpuTime - lastCpuTime;
        // 计算时间间隔
        TimeSpan timeDiff = currentTime - lastUpdateTime;
        // 计算CPU使用率
        float cpuUsage = (float) cpuTimeDiff.Ticks / (float) timeDiff.Ticks / (float) Environment.ProcessorCount;

        string cpuUsageString = $"{cpuUsage * 100:0.00}%";
        return cpuUsageString;
    }
}