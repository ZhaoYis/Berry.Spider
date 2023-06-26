using Volo.Abp.Domain.Entities;

namespace Berry.Spider.EventBus.MongoDB;

public class CapPublishedMessage : Entity<long>
{
    public string Version { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Content { get; set; } = default!;

    public DateTime Added { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int Retries { get; set; }

    public string StatusName { get; set; } = default!;
}