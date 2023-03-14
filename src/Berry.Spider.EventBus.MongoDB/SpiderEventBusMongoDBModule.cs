using Berry.Spider.Contracts;
using Berry.Spider.EventBus.MongoDB.MongoDB;
using Berry.Spider.EventBus.MongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace Berry.Spider.EventBus.MongoDB;

[DependsOn(typeof(AbpMongoDbModule))]
public class SpiderEventBusMongoDBModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        MongoDBOptions? mongoDbOptions = configuration.GetSection(nameof(MongoDBOptions)).Get<MongoDBOptions>();
        if (mongoDbOptions is { })
        {
            Configure<AbpDbConnectionOptions>(options =>
            {
                string connectionString = mongoDbOptions.ConnectionString;
                options.ConnectionStrings.Default = connectionString;
            });

            context.Services.AddMongoDbContext<CapMongoDbContext>(options =>
            {
                options.AddDefaultRepositories();
                options.AddRepository<ICapPublishedMessageRepository, CapPublishedMessageRepository>();
                options.AddRepository<ICapReceivedMessageRepository, CapReceivedMessageRepository>();
            });
        }
    }
}