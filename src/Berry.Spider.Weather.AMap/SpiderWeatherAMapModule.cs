using Berry.Spider.Weather.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Berry.Spider.Weather.AMap;

[DependsOn(typeof(SpiderWeatherAbstractionsModule))]
public class SpiderWeatherAMapModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AMapOptions>(configuration.GetSection($"{nameof(WeatherOptions)}:{nameof(AMapOptions)}"));
        context.Services.AddTransient<IWeatherServce, AMapWeatherServce>();
        context.Services.AddHttpClient<AMapHttpClient>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<SpiderWeatherAMapModule>(); });
    }
}