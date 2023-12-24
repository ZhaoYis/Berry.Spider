using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.SyncToRedis;

/// <summary>
/// 同步数据库已存在的标题到redis，判重使用
/// </summary>
public interface ISyncToRedisAppService : ITransientDependency
{
    Task RunAsync();
}