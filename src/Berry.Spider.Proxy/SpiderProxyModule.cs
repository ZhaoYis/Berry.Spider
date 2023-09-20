using Berry.Spider.Proxy.QgNet;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.Proxy;

/// <summary>
/// 代理池模块
/// </summary>
[DependsOn(typeof(SpiderDefaultProxyModule), typeof(SpiderQgNetProxyModule))]
public class SpiderProxyModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<ISpiderProxyFactory, SpiderProxyFactory>();
    }
}