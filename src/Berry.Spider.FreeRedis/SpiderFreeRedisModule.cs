using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.FreeRedis;

public class SpiderFreeRedisModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<RedisOptions>(configuration.GetSection("Redis"));

        return Task.CompletedTask;
    }
}