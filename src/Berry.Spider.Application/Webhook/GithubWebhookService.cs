using Berry.Spider.Webhook;
using MediatR;

namespace Berry.Spider.Application.Webhook;

public class GithubWebhookService : IGithubWebhookService
{
    private readonly IMediator _mediator;

    public GithubWebhookService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// 处理webhook消息
    /// </summary>
    /// <returns></returns>
    public async Task HandleAsync(GithubWebhookDto body)
    {
        await _mediator.Publish(body);
    }
}