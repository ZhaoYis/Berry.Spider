using Berry.Spider.Core;
using Berry.Spider.EventBus.MongoDB.Repositories;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Application.Spider;

public class SpiderPubAndRecAppService : ApplicationService, ISpiderPubAndRecAppService
{
    private readonly ICapPublishedMessageRepository _publishedMessageRepository;
    private readonly ICapReceivedMessageRepository _receivedMessageRepository;

    public SpiderPubAndRecAppService(ICapPublishedMessageRepository publishedMessageRepository,
        ICapReceivedMessageRepository receivedMessageRepository)
    {
        _publishedMessageRepository = publishedMessageRepository;
        _receivedMessageRepository = receivedMessageRepository;
    }

    /// <summary>
    /// 清理待执行任务数据
    /// </summary>
    /// <returns></returns>
    public async Task ClearTodoTaskAsync(SpiderSourceFrom from)
    {
        List<string> names = from.TryGetRoutingKeys();

        await _publishedMessageRepository.BatchDeleteByNamesAsync(names, CancellationToken.None);
        await _receivedMessageRepository.BatchDeleteByNamesAsync(names, CancellationToken.None);
    }
}