using System.Threading.Tasks;
using Berry.Spider.Core;
using Berry.Spider.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Consumers;

public class WeatherLoadWorker : AsyncPeriodicBackgroundWorkerBase
{
    public WeatherLoadWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        Timer.Period = 2 * 60 * 60 * 1000;
        //Timer.Period = 10 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        IWeatherAppService weatherAppService = workerContext.ServiceProvider.GetRequiredService<IWeatherAppService>();
        IOptions<AMapAdcodeOptions> options = workerContext.ServiceProvider.GetRequiredService<IOptions<AMapAdcodeOptions>>();
        AMapAdcodeOptions mapAdcodeOptions = options.Value;
        if (mapAdcodeOptions is { Items: { Count: > 0 } })
        {
            foreach (NameValue<string> item in mapAdcodeOptions.Items)
            {
                await weatherAppService.GetAndSaveAsync(item.Name, item.Value, item.Name);
                await Task.Delay(50);
            }
        }
    }
}