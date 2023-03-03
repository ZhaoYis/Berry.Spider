using Berry.Spider.Contracts;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
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
            if (mongoDbOptions is { })
            {
                opt.UseMongoDB(o =>
                {
                    o.DatabaseConnection = mongoDbOptions.ConnectionString;
                    o.DatabaseName = "cap";
                    o.ReceivedCollection = "cap.received";
                    o.PublishedCollection = "cap.published";
                });
            }

            //使用管理面板
            opt.UseDashboard();

            //注册节点到Consul
            ConsulOptions? consulOptions = configuration.GetSection(nameof(ConsulOptions)).Get<ConsulOptions>();
            if (consulOptions is {IsEnabled: true})
            {
                string nodeId = Guid.NewGuid().ToString("N");
                string nodeName = "Consumer_" + nodeId;
                opt.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = consulOptions.DiscoveryServerHostName;
                    d.DiscoveryServerPort = consulOptions.DiscoveryServerPort;
                    d.NodeId = nodeId;
                    d.NodeName = nodeName;
                    d.CustomTags = new[] {"Berry_Spider_Consumer"};
                });
            }

            //消费者线程并行处理消息的线程数
            opt.ConsumerThreadCount = 5;
            //如果设置为 true，则每个消费者组都会根据 ConsumerThreadCount 设置的值创建单独的线程进行处理。
            opt.UseDispatchingPerGroup = true;
            //失败消息的过期时间（秒）
            opt.FailedMessageExpiredAfter = 30 * 24 * 3600;
            //成功消息的过期时间（秒）
            opt.SucceedMessageExpiredAfter = 7 * 24 * 3600;

            EventBusRabbitMqOptions? rabbitMqOptions =
                configuration.GetSection("RabbitMQ:Connections:Default").Get<EventBusRabbitMqOptions>();
            if (rabbitMqOptions is { })
            {
                opt.UseRabbitMQ(o =>
                {
                    o.HostName = rabbitMqOptions.HostName;
                    o.UserName = rabbitMqOptions.UserName;
                    o.Password = rabbitMqOptions.Password;
                    o.Port = Convert.ToInt32(rabbitMqOptions.Port);
                    o.VirtualHost = rabbitMqOptions.VirtualHost;

                    o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                    {
                        new(Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                        new(Headers.MessageName, e.RoutingKey)
                    };
                });
            }
        }).AddSubscribeFilter<CustomSubscribeFilter>();

        //注册RabbitMq消息发布器
        context.Services.AddTransient<IEventBusPublisher, RabbitMqEventBusPublisher>();
    }
}