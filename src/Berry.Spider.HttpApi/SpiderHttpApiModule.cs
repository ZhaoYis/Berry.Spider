using Berry.Spider.Baidu;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Berry.Spider;

[DependsOn(
    //头条模块服务
    typeof(TouTiaoSpiderContractsModule),
    //百度模块服务
    typeof(BaiduSpiderContractsModule),
    //搜狗模块服务
    typeof(SogouSpiderContractsModule),
    //爬虫模块服务
    typeof(SpiderContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class SpiderHttpApiModule : AbpModule
{
    public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SpiderHttpApiModule).Assembly);
        });

        return Task.CompletedTask;
    }
}