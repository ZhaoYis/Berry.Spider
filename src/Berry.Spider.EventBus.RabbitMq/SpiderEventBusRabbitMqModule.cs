using Berry.Spider.EntityFrameworkCore;
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

        EventBusRabbitMqOptions options = configuration.GetSection("RabbitMQ:Connections:Default").Get<EventBusRabbitMqOptions>();
        context.Services.AddCap(opt =>
        {
            opt.UseEntityFramework<SpiderDbContext>();
            opt.UseDashboard();
            //消费者线程并行处理消息的线程数
            opt.ConsumerThreadCount = 5;
            //如果设置为 true，则每个消费者组都会根据 ConsumerThreadCount 设置的值创建单独的线程进行处理。
            opt.UseDispatchingPerGroup = true;
            //失败消息的过期时间（秒）
            opt.FailedMessageExpiredAfter = 30 * 24 * 3600;
            //成功消息的过期时间（秒）
            opt.SucceedMessageExpiredAfter = 7 * 24 * 3600;

            opt.UseRabbitMQ(o =>
            {
                o.HostName = options.HostName;
                o.UserName = options.UserName;
                o.Password = options.Password;
                o.Port = Convert.ToInt32(options.Port);
                o.VirtualHost = options.VirtualHost;

                o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                {
                    new(Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                    new(Headers.MessageName, e.RoutingKey)
                };
            });
        }).AddSubscribeFilter<CustomSubscribeFilter>();

        //注册RabbitMq消息发布器
        context.Services.AddTransient<IEventBusPublisher, RabbitMqEventBusPublisher>();
    }
}