using System.Text.Json.Serialization;
using Berry.Spider.Core;
using MediatR;

namespace Berry.Spider.Webhook;

/// <summary>
/// 详细实体信息参考：doc/github_webhook/released.json
/// </summary>
public class GithubWebhookDto : INotification
{
    [JsonPropertyName("action")] public GithubWebhookAction Action { get; set; }

    [JsonPropertyName("release")] public GithubWebhookReleaseDto Release { get; set; }
}

public class GithubWebhookReleaseDto
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("tag_name")] public string TagName { get; set; }

    [JsonPropertyName("target_commitish")] public string TargetCommitish { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("published_at")] public DateTime PublishedAt { get; set; }
}