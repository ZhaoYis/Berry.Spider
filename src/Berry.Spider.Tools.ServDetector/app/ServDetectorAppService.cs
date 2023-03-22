using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.ServDetector;

/// <summary>
/// 服务探测器
/// </summary>
public class ServDetectorAppService : IServDetectorAppService
{
    private ILogger<ServDetectorAppService> Logger { get; }

    public ServDetectorAppService(ILogger<ServDetectorAppService> logger)
    {
        Logger = logger;
    }

    public virtual async Task RunAsync()
    {
        Console.WriteLine("ServDetectorAppService start.");
    }
}