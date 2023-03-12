using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Berry.Spider.EventBus.MongoDB.MongoDB;

[ConnectionStringName("Default")]
public class CapMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<CapPublishedMessage> CapPublishedMessages => Collection<CapPublishedMessage>();
    public IMongoCollection<CapReceivedMessage> CapReceivedMessages => Collection<CapReceivedMessage>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.Entity<CapPublishedMessage>(b =>
        {
            b.CollectionName = "cap.published";
        });

        modelBuilder.Entity<CapReceivedMessage>(b =>
        {
            b.CollectionName = "cap.received";
        });
    }
}