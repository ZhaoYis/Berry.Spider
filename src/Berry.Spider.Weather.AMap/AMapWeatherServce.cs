using Berry.Spider.Weather.Abstractions;
using Berry.Spider.Weather.Shared;

namespace Berry.Spider.Weather.AMap;

/// <summary>
/// 基于高德地图查询天气预报数据
/// </summary>
public class AMapWeatherServce : IWeatherServce
{
    private readonly AMapHttpClient _aMapHttpClient;

    public AMapWeatherServce(AMapHttpClient aMapHttpClient)
    {
        _aMapHttpClient = aMapHttpClient;
    }

    /// <summary>
    /// 根据城市编码获取当前城市的天气预报数据
    /// </summary>
    /// <param name="province">省份名称</param>
    /// <param name="adcode">城市编码</param>
    /// <param name="city">城市名称</param>
    /// <remarks>根据实际对接的天气服务商预定的编码来进行传递，然后再在具体服务中进行处理</remarks>
    /// <returns></returns>
    public async Task<WeatherForecastDTO> GetWeatherForecastAsync(string province, string adcode, string city)
    {
        AMapWeatherInfoDTO? result = await _aMapHttpClient.GetWeatherInfoAsync(adcode);
        //TODO:对象转换
        return null;
    }
}