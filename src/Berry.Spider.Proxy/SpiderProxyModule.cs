using Microsoft.Extensions.DependencyInjection;
using Polly;
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
        var retryForeverPolicy = Policy<HttpResponseMessage>.Handle<Exception>().WaitAndRetryForeverAsync(
            sleepDurationProvider:i => TimeSpan.FromSeconds(2 * i)
        );
        context.Services.AddHttpClient<ProxyPoolHttpClient>().AddPolicyHandler(retryForeverPolicy);

        //配置HttpProxyOptions
        context.Services.Configure<HttpProxyOptions>(configuration.GetSection(nameof(HttpProxyOptions)));

        return Task.CompletedTask;
    }
}