using Berry.Spider.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Contracts;

[DependsOn(
    typeof(SpiderDomainSharedModule)
)]
public class SpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //配置WebDriverOptions
        context.Services.Configure<WebDriverOptions>(configuration.GetSection(nameof(WebDriverOptions)));

        return Task.CompletedTask;
    }
}