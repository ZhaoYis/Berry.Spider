using System.Text.Json;
using Berry.Spider.Common;
using Berry.Spider.FreeRedis;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace Berry.Spider.Application.Spider;

public class SpiderLifetimeAppService : ApplicationService, ISpiderLifetimeAppService
{
    private readonly IRedisService _redisService;
    private readonly IObjectMapper _mapper;

    public SpiderLifetimeAppService(IRedisService redisService, IObjectMapper mapper)
    {
        _redisService = redisService;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取爬虫服务状态
    /// </summary>
    /// <returns></returns>
    public async Task<List<ApplicationLifetimeDto>> GetSpiderStatusAsync()
    {
        var lifetimeDict = await _redisService.HGetAllAsync<string>(AppGlobalConstants.SPIDER_APPLICATION_LIFETIME_KEY);
        if (lifetimeDict is { Count: > 0 })
        {
            List<ApplicationLifetimeDto> applicationLifetimeList = new();
            foreach (KeyValuePair<string, string> pair in lifetimeDict)
            {
                string json = pair.Value;

                ApplicationLifetimeData? lifetime = JsonSerializer.Deserialize<ApplicationLifetimeData>(json);
                if (lifetime is { })
                {
                    ApplicationLifetimeDto dto = _mapper.Map<ApplicationLifetimeData, ApplicationLifetimeDto>(lifetime);
                    dto.AreYouOk = lifetime is { AreYouOk: true, IsKill: false } && !lifetime.IsOverTime();

                    applicationLifetimeList.Add(dto);
                }
            }

            applicationLifetimeList = applicationLifetimeList.OrderBy(c => c.MachineName).ToList();

            return applicationLifetimeList;
        }

        return default;
    }
}