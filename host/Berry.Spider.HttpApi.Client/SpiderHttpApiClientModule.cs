using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Berry.Spider.HttpApi;

[DependsOn(
    typeof(SpiderContractsModule),
    typeof(AbpHttpClientModule))]
public class SpiderHttpApiClientModule : AbpModule
{
    private const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Dynamic C# API Client Proxies.
        // context.Services.AddHttpClientProxies(
        //     typeof(SpiderContractsModule).Assembly,
        //     RemoteServiceName
        // );

        //Static C# API Client Proxies.
        context.Services.AddStaticHttpClientProxies(
            typeof(SpiderContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<SpiderHttpApiClientModule>(); });
    }
}