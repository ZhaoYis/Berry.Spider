using Berry.Spider.AspNetCore.Mvc;
using Berry.Spider.Webhook;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Berry.Spider.Webhooks;

/// <summary>
/// github的webhook
/// </summary>
[DisableDataWrapper]
[Route("api/services/github/webhooks")]
public class GithubWebhookController : AbpControllerBase
{
    private IGithubWebhookService GithubWebhookService { get; }

    public GithubWebhookController(IGithubWebhookService githubWebhookService)
    {
        this.GithubWebhookService = githubWebhookService;
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    [HttpPost, Route("receive")]
    public async Task ReceiveAsync([FromBody] GithubWebhookDto body)
    {
        await this.GithubWebhookService.HandleAsync(body);
    }
}