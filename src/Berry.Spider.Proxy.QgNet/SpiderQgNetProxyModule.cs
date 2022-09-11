using Berry.Spider.Proxy.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Volo.Abp.Modularity;

namespace Berry.Spider.Proxy.QgNet;

/// <summary>
/// https://www.qg.net代理池
/// </summary>
[DependsOn(typeof(SpiderProxyAbstractionsModule))]
public class SpiderQgNetProxyModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //注册ProxyPoolHttpClient
        var retryForeverPolicy = Policy<HttpResponseMessage>.Handle<Exception>().WaitAndRetryForeverAsync(
            sleepDurationProvider: i => TimeSpan.FromSeconds(2 * i)
        );
        context.Services.AddHttpClient<QgNetProxyHttpClient>().AddPolicyHandler(retryForeverPolicy);

        //配置QgNetProxyOptions
        context.Services.Configure<QgNetProxyOptions>(configuration.GetSection(nameof(QgNetProxyOptions)));

        return Task.CompletedTask;
    }
}