using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.NaiPan;

/// <summary>
/// http://www.naipan.com/api_webservice.html
/// </summary>
public class SpiderNaiPanModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<NaiPanOptions>(context.Services.GetConfiguration().GetSection(nameof(NaiPanOptions)));

        context.Services.AddSingleton<INaiPanService, NaiPanService>();
    }
}