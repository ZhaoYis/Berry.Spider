using Berry.Spider.TouTiao.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Berry.Spider;

[DependsOn(
    typeof(TouTiaoSpiderContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class SpiderHttpApiModule : AbpModule
{
    public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SpiderHttpApiModule).Assembly);
        });

        return Task.CompletedTask;
    }
}