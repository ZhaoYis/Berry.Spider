using Berry.Spider.Application.Contracts;
using Berry.Spider.Core;
using Berry.Spider.EventBus;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：汉语
/// </summary>
[SpiderService(new[] { SpiderSourceFrom.Sogou_HanYu })]
public class SogouSpider4HanYuProvider : ProviderBase<SogouSpider4HanYuProvider>, ISpiderProvider
{
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://hanyu.sogou.com";

    public SogouSpider4HanYuProvider(IOptionsSnapshot<SpiderOptions> options,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        ILogger<SogouSpider4HanYuProvider> logger) : base(logger)
    {
        this.Options = options.Value;
        this.RedisService = redisService;
        this.DistributedEventBus = eventBus;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync(SpiderPushToQueueDto dto)
    {
        string identityId = dto.GetIdentityId();
        var eto = dto.SourceFrom.TryCreateEto(EtoType.Push, dto.SourceFrom, dto.Keyword, dto.TraceCode, identityId);

        await this.CheckAsync(identityId, dto.SourceFrom, async () =>
            {
                string topicName = eto.TryGetRoutingKey();
                await this.DistributedEventBus.PublishAsync(topicName, eto);
            },
            bloomCheck: this.Options.KeywordCheckOptions.BloomCheck,
            duplicateCheck: this.Options.KeywordCheckOptions.RedisCheck);
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected override async Task<bool> DuplicateCheckAsync(string keyword, SpiderSourceFrom from)
    {
        string key = AppGlobalConstants.SPIDER_KEYWORDS_KEY;
        if (this.Options.KeywordCheckOptions.OnlyCurrentCategory)
        {
            key += $":{from.GetName()}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <returns></returns>
    public async Task HandlePushEventAsync<T>(T eventData) where T : class, ISpiderPushEto
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        throw new NotImplementedException();
    }
}