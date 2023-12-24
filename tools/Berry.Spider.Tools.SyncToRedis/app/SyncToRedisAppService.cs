using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.Logging;
using Volo.Abp.Uow;

namespace Berry.Spider.Tools.SyncToRedis;

/// <summary>
/// 同步数据库已存在的标题到redis，判重使用
/// </summary>
public class SyncToRedisAppService : ISyncToRedisAppService
{
    private SpiderContentDapperRepository SpiderContentDapperRepository { get; }
    private IRedisService RedisService { get; }
    private ILogger<SyncToRedisAppService> Logger { get; }

    public SyncToRedisAppService(SpiderContentDapperRepository dapperRepository,
        IRedisService redisService,
        ILogger<SyncToRedisAppService> logger)
    {
        this.SpiderContentDapperRepository = dapperRepository;
        this.RedisService = redisService;
        this.Logger = logger;
    }

    [UnitOfWork]
    public virtual async Task RunAsync()
    {
        IEnumerable<SpiderContent> contents = await this.SpiderContentDapperRepository.GetAllAsync();
        foreach (SpiderContent content in contents)
        {
            await this.RedisService.SetAsync(AppGlobalConstants.SPIDER_KEYWORDS_KEY, content.Title);
        }
    }
}