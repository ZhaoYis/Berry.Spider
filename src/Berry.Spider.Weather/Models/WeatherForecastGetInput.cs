namespace Berry.Spider.Weather;

public class WeatherForecastGetInput
{
    /// <summary>
    /// 省份名称
    /// </summary>
    public string Province { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    public string Adcode { get; set; }

    /// <summary>
    /// 城市名称
    /// </summary>
    public string City { get; set; }
}