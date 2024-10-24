using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Volo.Abp.Guids;

namespace Berry.Spider.Consumers;

public class SpiderClientRegister(IGuidGenerator guidGenerator, IRedisService redisService) : ISpiderClientRegister
{
    /// <summary>
    /// 服务注册
    /// </summary>
    /// <returns></returns>
    public async Task RegisterAsync()
    {
        try
        {
            //当前启动程序的进程id
            int pid = Environment.ProcessId;
            string clientId = guidGenerator.Create().ToString("N");

            ApplicationProcessData applicationProcess = new ApplicationProcessData { ClientId = clientId, Pid = pid };
            AppDomain.CurrentDomain.SetData(AppGlobalConstants.SPIDER_CLIENT_KEY, applicationProcess);

            //注册服务
            ApplicationLifetimeData lifetime = new ApplicationLifetimeData { AreYouOk = true };
            await redisService.HSetAsync(AppGlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, clientId, JsonSerializer.Serialize(lifetime));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// 取消服务注册
    /// </summary>
    /// <returns></returns>
    public async Task UnRegisterAsync()
    {
        try
        {
            object? applicationProcess = AppDomain.CurrentDomain.GetData(AppGlobalConstants.SPIDER_CLIENT_KEY);
            if (applicationProcess is ApplicationProcessData app)
            {
                string json = await redisService.HGetAsync<string>(AppGlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, app.ClientId);
                ApplicationLifetimeData? lifetime = JsonSerializer.Deserialize<ApplicationLifetimeData>(json);
                if (lifetime is { })
                {
                    lifetime.IsKill = true;
                    lifetime.AreYouOk = false;
                    await redisService.HSetAsync(AppGlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY, app.ClientId, JsonSerializer.Serialize(lifetime));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}