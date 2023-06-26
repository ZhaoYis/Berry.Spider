// using MongoDB.Driver;
// using Volo.Abp.DependencyInjection;
// using Volo.Abp.Domain.Entities;
// using Volo.Abp.Domain.Repositories.MongoDB;
//
// namespace Berry.Spider.EventBus.MongoDB.Repositories;
//
// public class CustomMongoDbBulkOperationProvider : IMongoDbBulkOperationProvider, ITransientDependency
// {
//     public async Task InsertManyAsync<TEntity>(IMongoDbRepository<TEntity> repository, IEnumerable<TEntity> entities, IClientSessionHandle sessionHandle, bool autoSave, CancellationToken cancellationToken)
//         where TEntity : class, IEntity
//     {
//     }
//
//     public async Task UpdateManyAsync<TEntity>(IMongoDbRepository<TEntity> repository, IEnumerable<TEntity> entities, IClientSessionHandle sessionHandle, bool autoSave, CancellationToken cancellationToken)
//         where TEntity : class, IEntity
//     {
//     }
//
//     public async Task DeleteManyAsync<TEntity>(IMongoDbRepository<TEntity> repository, IEnumerable<TEntity> entities, IClientSessionHandle sessionHandle, bool autoSave, CancellationToken cancellationToken)
//         where TEntity : class, IEntity
//     {
//     }
// }