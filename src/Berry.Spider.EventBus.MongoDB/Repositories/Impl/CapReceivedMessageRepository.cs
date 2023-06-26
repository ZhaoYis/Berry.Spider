using MongoDB.Driver;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Berry.Spider.EventBus.MongoDB.Repositories;

public class CapReceivedMessageRepository : MongoDbRepository<CapMongoDbContext, CapReceivedMessage, long>, ICapReceivedMessageRepository
{
    public CapReceivedMessageRepository(IMongoDbContextProvider<CapMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        await collection.DeleteOneAsync(
            Builders<CapReceivedMessage>.Filter.Eq(b => b.Id, id),
            cancellationToken
        );
    }

    public async Task BatchDeleteByNamesAsync(List<string> names, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        await collection.DeleteManyAsync(
            Builders<CapReceivedMessage>.Filter.In(b => b.Name, names),
            cancellationToken
        );
    }

    public async Task BatchDeleteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        await collection.DeleteManyAsync(
            Builders<CapReceivedMessage>.Filter.Eq(b => b.Name, name),
            cancellationToken
        );
    }
}