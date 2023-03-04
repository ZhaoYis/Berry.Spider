using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Consumers.HttpApi;

public class SpiderConsumerHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<ISpiderConsumerHttpApiService, SpiderConsumerHttpApiService>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}