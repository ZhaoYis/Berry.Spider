using System.Text.Json.Serialization;

namespace Berry.Spider.Weather.AMap;

public class AMapWeatherInfoDTO : AMapBase
{
    /// <summary>
    /// 预报天气信息数据
    /// </summary>
    [JsonPropertyName("forecasts")]
    public List<ForecastDTO> Forecasts { get; set; }
}

public class ForecastDTO
{
    /// <summary>
    /// 城市名称
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    [JsonPropertyName("adcode")]
    public string Adcode { get; set; }

    /// <summary>
    /// 省份名称
    /// </summary>
    [JsonPropertyName("province")]
    public string Province { get; set; }

    /// <summary>
    /// 预报发布时间
    /// </summary>
    [JsonPropertyName("reporttime")]
    public DateTime Reporttime { get; set; }

    /// <summary>
    /// 预报数据list结构，元素 cast,按顺序为当天、第二天、第三天的预报数据
    /// </summary>
    [JsonPropertyName("casts")]
    public List<CastDTO> Casts { get; set; }
}

public class CastDTO
{
    /// <summary>
    /// 日期
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// 星期几
    /// </summary>
    [JsonPropertyName("week")]
    public int Week { get; set; }

    /// <summary>
    /// 白天天气现象
    /// </summary>
    [JsonPropertyName("dayweather")]
    public string DayWeather { get; set; }

    /// <summary>
    /// 晚上天气现象
    /// </summary>
    [JsonPropertyName("nightweather")]
    public string NightWeather { get; set; }

    /// <summary>
    /// 白天温度
    /// </summary>
    [JsonPropertyName("daytemp")]
    public string DayTemp { get; set; }

    /// <summary>
    /// 晚上温度
    /// </summary>
    [JsonPropertyName("nighttemp")]
    public string NightTemp { get; set; }

    /// <summary>
    /// 白天风向
    /// </summary>
    [JsonPropertyName("daywind")]
    public string DayWind { get; set; }

    /// <summary>
    /// 晚上风向
    /// </summary>
    [JsonPropertyName("nightwind")]
    public string NightWind { get; set; }

    /// <summary>
    /// 白天风力
    /// </summary>
    [JsonPropertyName("daypower")]
    public string DayPower { get; set; }

    /// <summary>
    /// 晚上风力
    /// </summary>
    [JsonPropertyName("nightpower")]
    public string NightPower { get; set; }

    /// <summary>
    /// 白天温度（带小数）
    /// </summary>
    [JsonPropertyName("daytemp_float")]
    public decimal DayTempFloat { get; set; }

    /// <summary>
    /// 晚上温度（带小数）
    /// </summary>
    [JsonPropertyName("nighttemp_float")]
    public decimal NightTempFloat { get; set; }
}