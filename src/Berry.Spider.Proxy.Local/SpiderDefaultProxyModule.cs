using Berry.Spider.Proxy.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Volo.Abp.Modularity;

namespace Berry.Spider.Proxy;

/// <summary>
/// proxy pool开源项目ip代理池
/// </summary>
[DependsOn(typeof(SpiderProxyAbstractionsModule))]
public class SpiderDefaultProxyModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //注册ProxyPoolHttpClient
        var retryForeverPolicy = Policy<HttpResponseMessage>.Handle<Exception>().WaitAndRetryForeverAsync(
            sleepDurationProvider: i => TimeSpan.FromSeconds(2 * i)
        );
        context.Services.AddHttpClient<ProxyPoolHttpClient>().AddPolicyHandler(retryForeverPolicy);

        //配置HttpProxyOptions
        context.Services.Configure<HttpProxyOptions>(configuration.GetSection(nameof(HttpProxyOptions)));
        context.Services.AddScoped<IHttpProxy, DefaultHttpProxy>();
        
        return Task.CompletedTask;
    }
}