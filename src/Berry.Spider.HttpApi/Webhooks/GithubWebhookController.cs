using System.Text.Json;
using Berry.Spider.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Mvc;

namespace Berry.Spider.Webhooks;

/// <summary>
/// github的webhook
/// </summary>
[DisableDataWrapper]
[Route("api/services/github/webhooks")]
public class GithubWebhookController : AbpControllerBase
{
    [HttpPost, Route("receive")]
    public async Task ReceiveAsync([FromBody] object body)
    {
        this.Logger.LogInformation("--------webhooks receive---------");
        string jsonBody = JsonSerializer.Serialize(body);
        this.Logger.LogInformation(jsonBody);

        await Task.CompletedTask;
    }
}