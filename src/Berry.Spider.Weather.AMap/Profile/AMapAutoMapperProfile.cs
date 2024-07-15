using Berry.Spider.Weather.Shared;

namespace Berry.Spider.Weather.AMap.Profile;

public class AMapAutoMapperProfile : AutoMapper.Profile
{
    public AMapAutoMapperProfile()
    {
        CreateMap<ForecastDTO, WeatherForecastDTO>();
        CreateMap<CastDTO, WeatherCastDTO>();
    }
}