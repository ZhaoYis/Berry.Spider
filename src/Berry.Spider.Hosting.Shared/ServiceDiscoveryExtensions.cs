using Consul;

namespace Berry.Spider.Hosting.Shared;

public static class ServiceDiscoveryExtensions
{
    public static void AddConsulClient(this IServiceCollection service)
    {
        IConfiguration configuration = service.GetConfiguration();
        ConsulOptions? option = configuration.GetSection(nameof(ConsulOptions)).Get<ConsulOptions>();
        if (option is { Enabled: true })
        {
            service.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
            {
                //Consul主机地址
                if (!string.IsNullOrEmpty(option.Host))
                {
                    cfg.Address = new Uri(option.Host);
                }
            }));
        }
    }

    public static void UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            ConsulOptions? option = configuration.GetSection(nameof(ConsulOptions)).Get<ConsulOptions>();
            if (option is { Enabled: true })
            {
                string serviceId = Guid.NewGuid().ToString("N");
                string consulServiceId = $"{option.App.Name}:{serviceId}";

                var client = scope.ServiceProvider.GetRequiredService<IConsulClient>();
                string healthCheckPath = $"{option.App.Scheme}://{option.App.Host}:{option.App.Port.ToString()}/{option.HealthCheckPath}";

                //健康检查
                var httpCheck = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), //服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10), //间隔固定的时间访问一次
                    HTTP = healthCheckPath, //健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var agentServiceRegistration = new AgentServiceRegistration
                {
                    ID = consulServiceId,
                    Name = option.App.Name,
                    Address = option.App.Host,
                    Port = option.App.Port,
                    Tags = option.App.Tags,
                    Checks = new[] { httpCheck }
                };

                client.Agent.ServiceRegister(agentServiceRegistration).Wait();
                lifetime.ApplicationStopping.Register(() =>
                {
                    client.Agent.ServiceDeregister(agentServiceRegistration.ID).Wait();
                });
            }
        }
    }
}