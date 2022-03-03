using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Berry.Spider.Proxy;

public class SpiderProxyModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnApplicationInitializationAsync(context);
    }

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //注册ProxyPoolHttpClient
        context.Services.AddHttpClient<ProxyPoolHttpClient>();

        //配置HttpProxyOptions
        context.Services.Configure<HttpProxyOptions>(configuration.GetSection(nameof(HttpProxyOptions)));

        return Task.CompletedTask;
    }
}