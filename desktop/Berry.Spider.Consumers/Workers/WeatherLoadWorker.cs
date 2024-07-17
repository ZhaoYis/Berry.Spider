using System.Threading.Tasks;
using Berry.Spider.Weather;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Berry.Spider.Consumers;

public class WeatherLoadWorker : AsyncPeriodicBackgroundWorkerBase
{
    public WeatherLoadWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        //Timer.Period = 2 * 60 * 60 * 1000;
        Timer.Period = 10 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        IWeatherAppService weatherAppService = workerContext.ServiceProvider.GetRequiredService<IWeatherAppService>();
        await weatherAppService.GetAndSaveAsync("北京", "110000", "北京市");
    }
}