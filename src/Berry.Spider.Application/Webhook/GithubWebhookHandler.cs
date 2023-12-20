using Berry.Spider.Webhook;

namespace Berry.Spider.Application.Webhook;

public class GithubWebhookHandler : IGithubWebhookHandler
{
    /// <summary>
    /// 处理webhook消息
    /// </summary>
    /// <returns></returns>
    public Task HandleAsync(GithubWebhookDto body)
    {
        //TODO：处理消息
        return Task.CompletedTask;
    }
}