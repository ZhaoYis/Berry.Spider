using Berry.Spider.EventBus.MongoDB.MongoDB;
using MongoDB.Driver;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Berry.Spider.EventBus.MongoDB.Repositories;

public class CapPublishedMessageRepository : MongoDbRepository<CapMongoDbContext, CapPublishedMessage, long>, ICapPublishedMessageRepository
{
    public CapPublishedMessageRepository(IMongoDbContextProvider<CapMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        await collection.DeleteOneAsync(
            Builders<CapPublishedMessage>.Filter.Eq(b => b.Id, id),
            cancellationToken
        );
    }

    public async Task BatchDeleteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        await collection.DeleteManyAsync(
            Builders<CapPublishedMessage>.Filter.Eq(b => b.Name, name),
            cancellationToken
        );
    }
}