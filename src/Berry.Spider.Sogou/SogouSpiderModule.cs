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
        context.Services.AddSingleton<SogouSpider4RelatedSearchProvider>();

        //注入文本解析器
        context.Services.AddTransient<SogouRelatedSearchTextAnalysisProvider>();
    }
}