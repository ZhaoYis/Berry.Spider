using Berry.Spider.Domain;
using Berry.Spider.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Berry.Spider.EntityFrameworkCore;

public class WeatherForecastRepository : EfCoreRepository<SpiderDbContext, WeatherForecast, int>, IWeatherForecastRepository
{
    public WeatherForecastRepository(IDbContextProvider<SpiderDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}