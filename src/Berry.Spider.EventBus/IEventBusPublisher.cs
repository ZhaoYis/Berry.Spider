namespace Berry.Spider.EventBus;

public interface IEventBusPublisher
{
    /// <summary>
    /// 发布消息
    /// </summary>
    /// <returns></returns>
    Task PublishAsync<T>(string topicName, T body, CancellationToken cancellationToken = default(CancellationToken));
}