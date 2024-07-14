using Berry.Spider.Weather.AMap;
using Volo.Abp.Modularity;

namespace Berry.Spider.Weather;

[DependsOn(typeof(SpiderWeatherAMapModule))]
public class SpiderWeatherModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}