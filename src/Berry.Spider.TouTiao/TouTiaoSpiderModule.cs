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
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        //ע��ͷ�������ṩ��
        context.Services.AddTransient<TouTiaoSpider4QuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationCompositionProvider>();

        //ע���ı�������
        context.Services.AddTransient<TouTiaoQuestionTextAnalysisProvider>();

        //ע�������ʵ��ת��Url��ַ������
        context.Services.AddSingleton<TouTiaoResolveJumpUrlProvider>();

        return Task.CompletedTask;
    }
}