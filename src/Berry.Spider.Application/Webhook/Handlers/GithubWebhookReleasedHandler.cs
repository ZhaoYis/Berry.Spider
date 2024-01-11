using System.Text.Json;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Webhook;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Application;

/// <summary>
/// 处理Released通知消息
/// </summary>
public class GithubWebhookReleasedHandler : INotificationHandler<GithubWebhookDto>
{
    private SpiderAppDomainService SpiderAppDomainService { get; }
    private ILogger<GithubWebhookReleasedHandler> Logger { get; }

    public GithubWebhookReleasedHandler(SpiderAppDomainService spiderAppDomainService,
        ILogger<GithubWebhookReleasedHandler> logger)
    {
        this.SpiderAppDomainService = spiderAppDomainService;
        this.Logger = logger;
    }

    public async Task Handle(GithubWebhookDto body, CancellationToken cancellationToken)
    {
        if (body.Action == GithubWebhookAction.Released.GetName().ToLower())
        {
            this.Logger.LogInformation($"收到Github Webhook[{GithubWebhookAction.Released.GetName()}]消息，报文：{JsonSerializer.Serialize(body)}");

            GithubWebhookReleaseDto? releaseInfo = body.Release;
            if (releaseInfo is not null)
            {
                //包名称格式：20231220-v107
                //tag名称格式：20231220-v107
                string packageName = releaseInfo.Name;
                string tagNameName = releaseInfo.TagName;
                string targetCommitish = releaseInfo.TargetCommitish;

                await this.SpiderAppDomainService.CreateAppAsync(releaseInfo.Id,
                    packageName,
                    tagNameName,
                    targetCommitish,
                    releaseInfo.CreatedAt,
                    releaseInfo.PublishedAt
                );
            }
        }
    }
}