using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.EventBus.MongoDB.Repositories;

public interface ICapReceivedMessageRepository : IRepository<CapReceivedMessage, long>
{
    Task DeleteAsync(long id, CancellationToken cancellationToken);

    Task BatchDeleteByNameAsync(string name, CancellationToken cancellationToken);
    
    Task BatchDeleteByNamesAsync(List<string> names, CancellationToken cancellationToken);
}