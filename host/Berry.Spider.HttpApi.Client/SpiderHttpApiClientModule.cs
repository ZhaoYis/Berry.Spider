using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Berry.Spider.HttpApi;

[DependsOn(
    typeof(SpiderContractsModule))]
public class SpiderHttpApiClientModule : AbpModule
{
    private const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(SpiderContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<SpiderHttpApiClientModule>(); });
    }
}