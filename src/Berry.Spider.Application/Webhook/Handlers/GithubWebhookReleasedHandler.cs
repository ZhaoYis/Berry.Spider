using System.Text.Json;
using Berry.Spider.Core;
using Berry.Spider.Webhook;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Application.Webhook.Handlers;

/// <summary>
/// 处理Released通知消息
/// </summary>
public class GithubWebhookReleasedHandler : INotificationHandler<GithubWebhookDto>
{
    private ILogger<GithubWebhookReleasedHandler> Logger { get; }

    public GithubWebhookReleasedHandler(ILogger<GithubWebhookReleasedHandler> logger)
    {
        this.Logger = logger;
    }

    public async Task Handle(GithubWebhookDto body, CancellationToken cancellationToken)
    {
        if (body.Action == GithubWebhookAction.Released)
        {
            this.Logger.LogInformation($"收到[{GithubWebhookAction.Released.ToString()}]消息，报文：{JsonSerializer.Serialize(body)}");

            GithubWebhookReleaseDto releaseInfo = body.Release;
            //包名称格式：20231220-v107
            //tag名称格式：20231220-v107
            string version = releaseInfo.Name.Split('-').Last();
            string packageName = releaseInfo.Name;
            string tagNameName = releaseInfo.TagName;
        }
    }
}