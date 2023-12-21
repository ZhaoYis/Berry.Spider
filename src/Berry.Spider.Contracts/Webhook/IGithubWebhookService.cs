using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Webhook;

public interface IGithubWebhookService : ITransientDependency
{
    /// <summary>
    /// 处理webhook消息
    /// </summary>
    /// <returns></returns>
    Task HandleAsync(GithubWebhookDto body);
}