using Berry.Spider.Baidu;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Berry.Spider.HttpApi.Client;

[DependsOn(
    typeof(AbpHttpClientModule),
    typeof(SpiderApplicationContractsModule),
    typeof(TouTiaoSpiderApplicationContractsModule),
    typeof(BaiduSpiderApplicationContractsModule),
    typeof(SogouSpiderApplicationContractsModule)
)]
public class SpiderHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Dynamic C# API Client Proxies.
        // context.Services.AddHttpClientProxies(typeof(SpiderHttpApiClientModule).Assembly,
        //     AppGlobalConstants.RemoteServiceName
        // );

        //Static C# API Client Proxies.
        //abp generate-proxy -t csharp -u http://localhost:44306 --without-contracts -m berry_spider
        //web host：abp generate-proxy -t js -u http://localhost:44306 -m berry_spider
        context.Services.AddStaticHttpClientProxies(typeof(SpiderApplicationContractsModule).Assembly, AppGlobalConstants.RemoteServiceName);
        context.Services.AddStaticHttpClientProxies(typeof(TouTiaoSpiderApplicationContractsModule).Assembly, AppGlobalConstants.RemoteServiceName);
        context.Services.AddStaticHttpClientProxies(typeof(BaiduSpiderApplicationContractsModule).Assembly, AppGlobalConstants.RemoteServiceName);
        context.Services.AddStaticHttpClientProxies(typeof(SogouSpiderApplicationContractsModule).Assembly, AppGlobalConstants.RemoteServiceName);

        Configure<DynamicJavaScriptProxyOptions>(options => { options.DisableModule(AppGlobalConstants.ModelName); });
        //xxx-generate-proxy.json --> Build Action --> EmbeddedResource
        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<SpiderHttpApiClientModule>(); });
    }
}