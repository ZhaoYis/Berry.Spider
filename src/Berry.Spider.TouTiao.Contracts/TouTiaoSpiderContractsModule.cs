using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao.Contracts;

public class TouTiaoSpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}