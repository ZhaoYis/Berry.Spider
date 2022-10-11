using Volo.Abp.DependencyInjection;

namespace Berry.Spider.FreeRedis;

public interface IRedisService : ISingletonDependency
{
    Task<bool> SetAsync(string key, string source);
}