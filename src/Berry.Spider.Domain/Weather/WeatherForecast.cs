namespace Berry.Spider.Domain;

/// <summary>
/// 天气预报
/// </summary>
public class WeatherForecast : EntityBase
{
    protected WeatherForecast()
    {
    }

    /// <summary>
    /// 省份名称
    /// </summary>
    public string Province { get; set; }

    /// <summary>
    /// 城市名称
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    public string Adcode { get; set; }

    /// <summary>
    /// 报告时间
    /// </summary>
    public DateTime ReportTime { get; set; }

    /// <summary>
    /// 报告时间戳
    /// </summary>
    public long ReportTimeTicks { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 星期几
    /// </summary>
    public int Week { get; set; }

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