namespace Berry.Spider.Weather;

public interface IWeatherProvider
{
    /// <summary>
    /// 获取天气预报数据
    /// </summary>
    /// <returns></returns>
    Task<List<WeatherForecastByDate>> GetAsync(WeatherForecastGetInput input);
}  