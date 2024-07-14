using System.Net.Http.Json;
using JetBrains.Annotations;

namespace Berry.Spider.Weather.AMap;

public class AMapHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AMapOptions _options;

    public AMapHttpClient(HttpClient client, AMapOptions options)
    {
        client.BaseAddress = new Uri("https://restapi.amap.com");
        _httpClient = client;
        _options = options;
    }

    /// <summary>
    /// 获取天气预报
    /// </summary>
    /// <param name="adcode">高德定义的城市编码</param>
    /// <returns></returns>
    public async Task<AMapWeatherInfoDTO?> GetWeatherInfoAsync([NotNull] string adcode)
    {
        string uri = $"v3/weather/weatherInfo?key={_options.Key}&city={adcode}&extensions=all";
        AMapWeatherInfoDTO? result = await _httpClient.GetFromJsonAsync<AMapWeatherInfoDTO>(uri);
        return result;
    }
}