using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Sogou;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SogouSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule)
)]
public class SogouSpiderModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //注入搜狗爬虫提供者
        context.Services.AddTransient<SogouSpider4RelatedSearchProvider>();
        context.Services.AddTransient<SogouSpider4WenWenProvider>();

        //注册解析真实跳转的Url地址解析器
        context.Services.AddSingleton<SougouResolveJumpUrlProvider>();
        
        //注入文本解析器
        context.Services.AddTransient<SogouRelatedSearchTextAnalysisProvider>();
        context.Services.AddTransient<SogouSpider4WenWenTextAnalysisProvider>();
    }
}