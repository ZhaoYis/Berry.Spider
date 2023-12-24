using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.VirtualFileSystem;

namespace Berry.Spider.Admin;

[DependsOn(
    typeof(AdminApplicationContractsModule),
    typeof(AbpAccountHttpApiClientModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpTenantManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule)
)]
public class AdminHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Dynamic C# API Client Proxies.
        // context.Services.AddHttpClientProxies(typeof(AdminApplicationContractsModule).Assembly,
        //     AdminGlobalConstants.RemoteServiceName
        // );

        //Static C# API Client Proxies.
        //abp generate-proxy -t csharp -u http://localhost:44346 --without-contracts -m berry_admin
        //web host：abp generate-proxy -t js -u http://localhost:44346 -m berry_admin
        context.Services.AddStaticHttpClientProxies(typeof(AdminApplicationContractsModule).Assembly,
            AdminGlobalConstants.RemoteServiceName
        );

        Configure<DynamicJavaScriptProxyOptions>(options => { options.DisableModule(AdminGlobalConstants.ModelName); });
        //xxx-generate-proxy.json --> Build Action --> EmbeddedResource
        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<AdminHttpApiClientModule>(); });
    }
}