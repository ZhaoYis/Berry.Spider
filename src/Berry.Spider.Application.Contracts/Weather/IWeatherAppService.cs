using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Weather;

public interface IWeatherAppService : IApplicationService, ITransientDependency
{
    /// <summary>
    /// 获取天气预报并保存
    /// </summary>
    /// <returns></returns>
    Task<bool> GetAndSaveAsync(string province, string adcode, string city);
}