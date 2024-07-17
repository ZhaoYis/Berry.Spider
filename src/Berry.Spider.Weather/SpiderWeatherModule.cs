using Berry.Spider.Weather.AMap;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Weather;

[DependsOn(typeof(SpiderWeatherAMapModule))]
public class SpiderWeatherModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IWeatherProvider, DefaultWeatherProvider>();
    }
}