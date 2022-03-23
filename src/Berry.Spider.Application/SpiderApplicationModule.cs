using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Berry.Spider.Application;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(SpiderContractsModule),
    typeof(AbpAutoMapperModule))]
public class SpiderApplicationModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<SpiderApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SpiderApplicationModule>(validate: true);
        });

        return Task.CompletedTask;
    }
}