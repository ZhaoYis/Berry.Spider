using Berry.Spider.Baidu;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Berry.Spider;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(TouTiaoSpiderApplicationContractsModule),
    typeof(BaiduSpiderApplicationContractsModule),
    typeof(SogouSpiderApplicationContractsModule),
    typeof(SpiderApplicationContractsModule)
)]
public class SpiderHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder => { mvcBuilder.AddApplicationPartIfNotExists(typeof(SpiderHttpApiModule).Assembly); });
    }
}