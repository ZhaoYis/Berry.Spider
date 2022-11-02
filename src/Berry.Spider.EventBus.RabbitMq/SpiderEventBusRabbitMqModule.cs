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