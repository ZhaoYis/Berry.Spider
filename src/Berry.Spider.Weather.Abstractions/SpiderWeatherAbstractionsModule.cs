using Berry.Spider.Weather.Shared;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Weather.Abstractions;

[DependsOn(typeof(SpiderWeatherSharedModule))]
public class SpiderWeatherAbstractionsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<WeatherOptions>(configuration.GetSection(nameof(WeatherOptions)));
    }
}