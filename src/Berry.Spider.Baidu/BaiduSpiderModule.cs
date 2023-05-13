using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Baidu;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(BaiduSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule)
)]
public class BaiduSpiderModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //注入百度爬虫提供者
        context.Services.AddTransient<BaiduSpider4RelatedSearchProvider>();

        //注入文本解析器
        context.Services.AddSingleton<BaiduRelatedSearchTextAnalysisProvider>();

        //注册解析真实跳转的Url地址解析器
        context.Services.AddSingleton<BaiduResolveJumpUrlProvider>();
    }
}