using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Domain.Repositories;

public interface IWeatherForecastRepository : IRepository<WeatherForecast, int>
{
}