using Volo.Abp.Application.Services;

namespace Berry.Spider.Webhook;

public interface IGithubWebhookService : IApplicationService
{
    /// <summary>
    /// 处理webhook消息
    /// </summary>
    /// <returns></returns>
    Task HandleAsync(GithubWebhookDto body);
}