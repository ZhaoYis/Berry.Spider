using Berry.Spider.Weather.Abstractions;

namespace Berry.Spider.Weather.AMap;

public class AMapOptions : WeatherOptions
{
    /// <summary>
    /// 高德key
    /// </summary>
    public List<string> Keys { get; set; } = null!;
}