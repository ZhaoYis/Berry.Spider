using Berry.Spider.Weather.Shared;

namespace Berry.Spider.Weather.Abstractions;

/// <summary>
/// 公共天气预报数据获取服务
/// </summary>
public interface IWeatherServce
{
    /// <summary>
    /// 根据城市编码获取当前城市的天气预报数据
    /// </summary>
    /// <param name="province">省份名称</param>
    /// <param name="adcode">城市编码</param>
    /// <param name="city">城市名称</param>
    /// <remarks>根据实际对接的天气服务商预定的编码来进行传递，然后再在具体服务中进行处理</remarks>
    /// <returns></returns>
    Task<WeatherForecastDTO> GetWeatherForecastAsync(string province, string adcode, string city);
}