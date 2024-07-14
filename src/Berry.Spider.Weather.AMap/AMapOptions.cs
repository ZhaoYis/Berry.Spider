using Berry.Spider.Weather.Abstractions;

namespace Berry.Spider.Weather.AMap;

public class AMapOptions : WeatherOptions
{
    /// <summary>
    /// 高德key
    /// </summary>
    public string Key { get; set; } = null!;
}