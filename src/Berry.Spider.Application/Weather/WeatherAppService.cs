using Berry.Spider.Domain;
using Berry.Spider.Weather;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Application.Weather;

public class WeatherAppService : ApplicationService, IWeatherAppService
{
    private IWeatherProvider WeatherProvider { get; }
    private WeatherForecastDomainService WeatherForecastDomainService { get; }

    public WeatherAppService(IWeatherProvider provider, WeatherForecastDomainService domainService)
    {
        WeatherProvider = provider;
        WeatherForecastDomainService = domainService;
    }

    public async Task<bool> GetAndSaveAsync(string province, string adcode, string city)
    {
        WeatherForecastGetInput forecastGetInput = new WeatherForecastGetInput
        {
            Province = province,
            Adcode = adcode,
            City = city
        };
        List<WeatherForecastByDate> weatherForecastByDateList = await this.WeatherProvider.GetAsync(forecastGetInput);
        List<WeatherForecast> weatherForecasts = this.ObjectMapper.Map<List<WeatherForecastByDate>, List<WeatherForecast>>(weatherForecastByDateList);
        weatherForecasts.ForEach(wf => { wf.ReportTimeTicks = wf.ReportTime.Ticks; });
        return await this.WeatherForecastDomainService.BatchSaveAsync(weatherForecasts);
    }
}