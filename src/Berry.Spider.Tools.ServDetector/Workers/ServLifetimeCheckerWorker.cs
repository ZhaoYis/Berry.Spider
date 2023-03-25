using System.Text.Json;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Berry.Spider.Weixin.Work;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Tools.ServDetector;

public class ServLifetimeCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    private readonly IWeixinWorkRobotClient _weixinWorkRobotClient;
    private WeixinWorkRobotOptions RobotOptions { get; }

    public ServLifetimeCheckerWorker(AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IWeixinWorkRobotClient weixinWorkRobotClient,
        IOptions<WeixinWorkRobotOptions> options) : base(timer, serviceScopeFactory)
    {
        _weixinWorkRobotClient = weixinWorkRobotClient;
        RobotOptions = options.Value;

        Timer.Period = 5 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        IRedisService? redisService = workerContext.ServiceProvider.GetService<IRedisService>();
        if (redisService is { })
        {
            var lifetimeDict = await redisService.HGetAllAsync<string>(GlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY);
            if (lifetimeDict is { Count: > 0 })
            {
                List<ApplicationLifetimeData> applicationLifetimeList = new();
                foreach (KeyValuePair<string, string> pair in lifetimeDict)
                {
                    string json = pair.Value;
                    ApplicationLifetimeData? lifetime = JsonSerializer.Deserialize<ApplicationLifetimeData>(json);
                    if (lifetime is { })
                    {
                        applicationLifetimeList.Add(lifetime);
                    }
                }

                //构建消息
                string msg = ApplicationLifetimeHelper.Build(applicationLifetimeList);

                //发送消息
                MarkdownMessageDto markdownMessage = new MarkdownMessageDto(msg);
                await _weixinWorkRobotClient.SendAsync(this.RobotOptions.AppKey, markdownMessage);
            }
        }
    }
}