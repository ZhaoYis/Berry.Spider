using DotNetCore.CAP;
using Volo.Abp;

namespace Berry.Spider.EventBus.Redis;

public class RedisEventBusPublisher : IEventBusPublisher
{
    private ICapPublisher Publisher { get; }

    public RedisEventBusPublisher(ICapPublisher publisher)
    {
        this.Publisher = publisher;
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <returns></returns>
    public async Task PublishAsync<T>(string topicName, T body, CancellationToken cancellationToken = default(CancellationToken))
    {
        Check.NotNull(topicName, nameof(topicName));

        await this.Publisher.PublishAsync<T>(topicName, body, cancellationToken: cancellationToken);
    }
}