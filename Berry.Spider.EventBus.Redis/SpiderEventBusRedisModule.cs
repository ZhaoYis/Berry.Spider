using Berry.Spider.Core;
using Berry.Spider.FreeRedis;
using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.EventBus.Redis;

[DependsOn(typeof(SpiderEventBusModule))]
public class SpiderEventBusRedisModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        RedisOptions? redisOptions = context.Configuration.GetSection("Redis").Get<RedisOptions>();
        context.Services.AddCap(opt =>
        {
            //配置数据存储方式
            opt.UseSqlite(sqlite => { sqlite.ConnectionString = "Data Source=cap.db"; });
            //配置Redis
            if (redisOptions is not null)
            {
                opt.UseRedis(redisOptions.Configuration);
            }
            else
            {
                //使用默认配置
                opt.UseRedis();
            }

            //配置消费者
            this.ConfigureConsumer(opt, configuration);
            //使用管理面板
            opt.UseDashboard();
        }).AddSubscribeFilter<CustomSubscribeFilter>();

        //注册Redis消息发布器
        context.Services.AddTransient<IEventBusPublisher, RedisEventBusPublisher>();
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
            // opt.EnableConsumerPrefetch = consumerOptions.EnableConsumerPrefetch;
            opt.EnableSubscriberParallelExecute = consumerOptions.EnableConsumerPrefetch;
            //失败消息的过期时间（秒）
            opt.FailedMessageExpiredAfter = consumerOptions.FailedMessageExpiredAfter;
            //成功消息的过期时间（秒）
            opt.SucceedMessageExpiredAfter = consumerOptions.SucceedMessageExpiredAfter;
        }
    }
}