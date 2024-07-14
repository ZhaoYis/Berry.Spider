namespace Berry.Spider.Weather.Shared;

/// <summary>
/// 公共天气预报数据模型
/// </summary>
public class WeatherForecastDTO
{
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
    public string CityCode { get; set; }

    /// <summary>
    /// 报告时间
    /// </summary>
    public DateTime ReportTime { get; set; }
}