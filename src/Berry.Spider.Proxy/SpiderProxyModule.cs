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
        context.Services.AddHttpClient<ProxyPoolHttpClient>();
        context.Services.Configure<HttpProxyOptions>(opt =>
        {
            opt.ProxyPoolApiHost = "http://127.0.0.1:5010";
        });

        return base.ConfigureServicesAsync(context);
    }
}