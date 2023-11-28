using System;
using System.Collections.Generic;
using Berry.Spider.Contracts;
using DotNetCore.CAP;
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
            //配置数据存储方式
            this.ConfigureDatabase(opt, configuration);
            //注册节点到Consul
            this.ConfigureConsul(opt, configuration);
            //配置消费者
            this.ConfigureConsumer(opt, configuration);
            //配置RabbitMQ
            this.ConfigureRabbitMq(opt, configuration);
            //使用管理面板
            opt.UseDashboard();
        }).AddSubscribeFilter<CustomSubscribeFilter>();

        //注册RabbitMq消息发布器
        context.Services.AddTransient<IEventBusPublisher, RabbitMqEventBusPublisher>();
    }

    /// <summary>
    /// 配置RabbitMQ
    /// </summary>
    private void ConfigureRabbitMq(CapOptions opt, IConfiguration configuration)
    {
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
    }

    /// <summary>
    /// 配置消费者
    /// </summary>
    private void ConfigureConsumer(CapOptions opt, IConfiguration configuration)
    {
        ConsumerOptions? consumerOptions = configuration.GetSection(nameof(ConsumerOptions)).Get<ConsumerOptions>();
        if (consumerOptions is not null)
        {
            //消费者线程并行处理消息的线程数
            opt.ConsumerThreadCount = consumerOptions.ConsumerThreadCount;
            //如果设置为 true，则每个消费者组都会根据 ConsumerThreadCount 设置的值创建单独的线程进行处理。
            // opt.UseDispatchingPerGroup = true;
            opt.EnableConsumerPrefetch = consumerOptions.EnableConsumerPrefetch;
            //失败消息的过期时间（秒）
            opt.FailedMessageExpiredAfter = consumerOptions.FailedMessageExpiredAfter;
            //成功消息的过期时间（秒）
            opt.SucceedMessageExpiredAfter = consumerOptions.SucceedMessageExpiredAfter;
        }
    }

    /// <summary>
    /// 注册节点到Consul
    /// </summary>
    private void ConfigureConsul(CapOptions opt, IConfiguration configuration)
    {
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
    }

    /// <summary>
    /// 配置数据存储方式
    /// </summary>
    private void ConfigureDatabase(CapOptions opt, IConfiguration configuration)
    {
        this.UseMySQLDb(opt, configuration);
    }

    /// <summary>
    /// 使用mysql作为底板
    /// </summary>
    private void UseMySQLDb(CapOptions opt, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("EventBus");
        if (!string.IsNullOrEmpty(connectionString))
        {
            opt.UseMySql(o => { o.ConnectionString = connectionString; });
        }
        else
        {
            throw new ApplicationException("消息队列数据库底板未配置");
        }
    }

    /// <summary>
    /// 使用MongoDB作为底板
    /// </summary>
    private void UseMongoDb(CapOptions opt, IConfiguration configuration)
    {
        MongoDBOptions? mongoDbOptions = configuration.GetSection(nameof(MongoDBOptions)).Get<MongoDBOptions>();
        if (mongoDbOptions is not null and { IsEnabled: true })
        {
            opt.UseMongoDB(o =>
            {
                o.DatabaseConnection = mongoDbOptions.ConnectionString;
                o.DatabaseName = "cap";
                o.ReceivedCollection = "cap.received";
                o.PublishedCollection = "cap.published";
                o.LockCollection = "cap.lock";
            });

            opt.UseStorageLock = mongoDbOptions.UseStorageLock;
        }
        else
        {
            throw new ApplicationException("消息队列数据库底板未配置");
        }
    }

    /// <summary>
    /// 使用PostgreSQL作为底板
    /// </summary>
    private void UsePostgreSQL(CapOptions opt, IConfiguration configuration)
    {
        //TODO
    }
}