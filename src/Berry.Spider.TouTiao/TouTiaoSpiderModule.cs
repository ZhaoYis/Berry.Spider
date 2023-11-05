using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(SpiderCoreModule),
    typeof(SpiderDomainModule),
    typeof(SpiderAbstractionsModule),
    typeof(TouTiaoSpiderContractsModule)
)]
public class TouTiaoSpiderModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //注入头条爬虫提供者
        context.Services.AddTransient<TouTiaoSpider4QuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4HighQualityQuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationCompositionProvider>();

        //注入文本解析器
        context.Services.AddSingleton<TouTiaoQuestionTextAnalysisProvider>();

        //注册解析真实跳转的Url地址解析器
        context.Services.AddSingleton<TouTiaoResolveJumpUrlProvider>();

        //今日头条相关的独立配置信息
        context.Services.Configure<TouTiaoOptions>(configuration.GetSection(nameof(TouTiaoOptions)));
    }
}