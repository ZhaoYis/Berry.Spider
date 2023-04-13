using System.Text.Json;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Berry.Spider.Weixin;
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
        IOptionsSnapshot<WeixinWorkRobotOptions> options) : base(timer, serviceScopeFactory)
    {
        _weixinWorkRobotClient = weixinWorkRobotClient;
        RobotOptions = options.Value;

        Timer.Period = RobotOptions.TimerPeriod;
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
                List<string> todoRemoveNodes = new List<string>();
                foreach (KeyValuePair<string, string> pair in lifetimeDict)
                {
                    string clientId = pair.Key;
                    string json = pair.Value;

                    ApplicationLifetimeData? lifetime = JsonSerializer.Deserialize<ApplicationLifetimeData>(json);
                    if (lifetime is { })
                    {
                        applicationLifetimeList.Add(lifetime);

                        if (!lifetime.AreYouOk || lifetime.IsOverTime())
                        {
                            todoRemoveNodes.Add(clientId);
                        }
                    }
                }

                //构建消息
                string? msg = ApplicationLifetimeHelper.Build(applicationLifetimeList);
                if (!string.IsNullOrEmpty(msg))
                {
                    //发送消息
                    MarkdownMessageDto markdownMessage = new MarkdownMessageDto(msg);
                    WeixinResult weixinResult = await _weixinWorkRobotClient.SendAsync(this.RobotOptions.AppKey, markdownMessage);
                    if (weixinResult is { IsSuccessful: true })
                    {
                        //通知成功后清除节点信息
                        if (todoRemoveNodes is { Count: > 0 })
                        {
                            await redisService.HDelAsync(GlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, todoRemoveNodes.ToArray());
                        }
                    }
                    else
                    {
                        weixinResult.errmsg ??= "服务器未知错误";
                        TextMessageDto textMessage = new TextMessageDto(weixinResult.errmsg);
                        await _weixinWorkRobotClient.SendAsync(this.RobotOptions.AppKey, textMessage);
                    }
                }
            }
        }
    }
}