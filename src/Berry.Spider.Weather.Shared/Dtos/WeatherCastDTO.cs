namespace Berry.Spider.Weather.Shared;

/// <summary>
/// 预报数据。按天维度
/// </summary>
public class WeatherCastDTO
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 星期几
    /// </summary>
    public DayOfWeek Week { get; set; }
    
    /// <summary>
    /// 白天天气现象
    /// </summary>
    public string DayWeather { get; set; }

    /// <summary>
    /// 晚上天气现象
    /// </summary>
    public string NightWeather { get; set; }

    /// <summary>
    /// 白天风向
    /// </summary>
    public string DayWind { get; set; }

    /// <summary>
    /// 晚上风向
    /// </summary>
    public string NightWind { get; set; }

    /// <summary>
    /// 白天风力
    /// </summary>
    public string DayPower { get; set; }

    /// <summary>
    /// 晚上风力
    /// </summary>
    public string NightPower { get; set; }

    /// <summary>
    /// 白天温度（带小数）
    /// </summary>
    public decimal DayTempFloat { get; set; }

    /// <summary>
    /// 晚上温度（带小数）
    /// </summary>
    public decimal NightTempFloat { get; set; }
}