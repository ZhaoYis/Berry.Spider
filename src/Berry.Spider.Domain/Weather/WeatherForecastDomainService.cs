using Berry.Spider.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Berry.Spider.Domain;

public class WeatherForecastDomainService : DomainService
{
    private IWeatherForecastRepository WeatherForecastRepository { get; }

    public WeatherForecastDomainService(IWeatherForecastRepository weatherForecastRepository)
    {
        WeatherForecastRepository = weatherForecastRepository;
    }

    public async Task<bool> BatchSaveAsync(List<WeatherForecast> weatherForecasts)
    {
        //先检查数据是否存在了
        
        await this.WeatherForecastRepository.InsertManyAsync(weatherForecasts, autoSave: true);
        return true;
    }
}