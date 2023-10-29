using Berry.Spider.Contracts;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.EventBus.RabbitMq;

[DependsOn(typeof(SpiderEventBusModule))]
public class SpiderEventBusRabbitMqModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.AddCap(opt =>
        {
            MongoDBOptions? mongoDbOptions = configuration.GetSection(nameof(MongoDBOptions)).Get<MongoDBOptions>();
            if (mongoDbOptions is not null)
            {
                opt.UseMongoDB(o =>
                {
                    o.DatabaseConnection = mongoDbOptions.ConnectionString;
                    o.DatabaseName = "cap";
                    o.ReceivedCollection = "cap.received";
                    o.PublishedCollection = "cap.published";
                    //o.LockCollection = "cap.lock";
                });

                //opt.UseStorageLock = true;
            }

            //使用管理面板
            opt.UseDashboard();

            //注册节点到Consul
            ConsulOptions? consulOptions = configuration.GetSection(nameof(ConsulOptions)).Get<ConsulOptions>();
            if (consulOptions is { IsEnabled: true })
            {
                string nodeId = Guid.NewGuid().ToString("N");
                string nodeName = "Consumer_" + nodeId;
                opt.UseConsulDiscovery(d =>
                {
                    d.DiscoveryServerHostName = consulOptions.DiscoveryServerHostName;
                    d.DiscoveryServerPort = consulOptions.DiscoveryServerPort;
                    d.NodeId = nodeId;
                    d.NodeName = nodeName;
                    d.CustomTags = new[] { "Berry_Spider_Consumer" };
                });
            }

            ConsumerOptions? consumerOptions = configuration.GetSection(nameof(ConsumerOptions)).Get<ConsumerOptions>();
            if (consumerOptions is not null)
            {
                //消费者线程并行处理消息的线程数
                opt.ConsumerThreadCount = consumerOptions.ConsumerThreadCount;
                //如果设置为 true，则每个消费者组都会根据 ConsumerThreadCount 设置的值创建单独的线程进行处理。
                // opt.UseDispatchingPerGroup = true;
                opt.EnableConsumerPrefetch = consumerOptions.EnableConsumerPrefetch;
            }

            //失败消息的过期时间（秒）
            opt.FailedMessageExpiredAfter = 30 * 24 * 3600;
            //成功消息的过期时间（秒）
            opt.SucceedMessageExpiredAfter = 7 * 24 * 3600;

            EventBusRabbitMqOptions? rabbitMqOptions =
                configuration.GetSection("RabbitMQ:Connections:Default").Get<EventBusRabbitMqOptions>();
            if (rabbitMqOptions is not null)
            {
                opt.UseRabbitMQ(o =>
                {
                    o.HostName = rabbitMqOptions.HostName;
                    o.UserName = rabbitMqOptions.UserName;
                    o.Password = rabbitMqOptions.Password;
                    o.Port = Convert.ToInt32(rabbitMqOptions.Port);
                    o.VirtualHost = rabbitMqOptions.VirtualHost;

                    o.CustomHeadersBuilder = (eventArgs, provider) =>
                    {
                        ISnowflakeId snowflakeId = provider.GetRequiredService<ISnowflakeId>();
                        return new List<KeyValuePair<string, string>>
                        {
                            new(Headers.MessageId, snowflakeId.NextId().ToString()),
                            new(Headers.MessageName, eventArgs.RoutingKey)
                        };
                    };

                    o.PublishConfirms = rabbitMqOptions.PublishConfirms;
                });
            }
        }).AddSubscribeFilter<CustomSubscribeFilter>();

        //注册RabbitMq消息发布器
        context.Services.AddTransient<IEventBusPublisher, RabbitMqEventBusPublisher>();
    }
}