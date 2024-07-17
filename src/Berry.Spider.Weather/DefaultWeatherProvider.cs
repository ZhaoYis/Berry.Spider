using Berry.Spider.Weather.Abstractions;
using Berry.Spider.Weather.Shared;

namespace Berry.Spider.Weather;

public class DefaultWeatherProvider : IWeatherProvider
{
    private IEnumerable<IWeatherServce> WeatherServces { get; }

    public DefaultWeatherProvider(IEnumerable<IWeatherServce> weatherServces)
    {
        this.WeatherServces = weatherServces;
    }

    public async Task<List<WeatherForecastByDate>> GetAsync(WeatherForecastGetInput input)
    {
        //TODO:这里后续使用相关策略来选择具体的服务
        IWeatherServce weatherServce = this.WeatherServces.First();
        List<WeatherForecastDTO>? weatherForecastList = await weatherServce.GetWeatherForecastAsync(input.Province, input.Adcode, input.City);

        List<WeatherForecastByDate> weatherForecastByDateList = new List<WeatherForecastByDate>();
        if (weatherForecastList is { Count: > 0 })
        {
            foreach (WeatherForecastDTO wf in weatherForecastList)
            {
                List<WeatherCastDTO> weatherCastList = wf.Casts;
                if (weatherCastList is { Count: > 0 })
                {
                    foreach (WeatherCastDTO cast in weatherCastList)
                    {
                        weatherForecastByDateList.Add(new WeatherForecastByDate
                        {
                            Province = wf.Province,
                            Adcode = wf.Adcode,
                            City = wf.City,
                            ReportTime = wf.ReportTime,
                            Date = cast.Date,
                            Week = cast.Week,
                            DayWeather = cast.DayWeather,
                            NightWeather = cast.NightWeather,
                            DayWind = cast.DayWind,
                            NightWind = cast.NightWind,
                            DayPower = cast.DayPower,
                            NightPower = cast.NightPower,
                            DayTempFloat = cast.DayTempFloat,
                            NightTempFloat = cast.NightTempFloat
                        });
                    }
                }
            }
        }

        return weatherForecastByDateList;
    }
}