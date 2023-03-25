using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Tools.ServDetector;

public class ServLifetimeCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ServLifetimeCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
    {
        Timer.Period = 5 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var provider = workerContext.ServiceProvider;

        Process[] processes = Process.GetProcesses();

        Console.WriteLine("PID\tProcess Name\tMemory Usage\tCPU Usage");
        foreach (Process process in processes.Where(c => c.ProcessName.StartsWith("Berry")))
        {
            string memoryUsage = string.Format("{0:#,##0} M", process.WorkingSet64 / 1024 / 1024);
            string cpuUsage = "";
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                cpuUsage = "None";
            }
            else
            {
                cpuUsage = string.Format("{0:0.00}%", process.TotalProcessorTime / Process.GetCurrentProcess().TotalProcessorTime * 100);
            }

            Console.WriteLine("{0}\t{1}\t{2}\t{3}", process.Id, process.ProcessName, memoryUsage, cpuUsage);
        }
    }
}