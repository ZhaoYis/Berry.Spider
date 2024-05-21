using Berry.Spider.Baidu;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.EventBus.RabbitMq;
using Berry.Spider.FreeRedis;
using Berry.Spider.NaiPan;
using Berry.Spider.RealTime;
using Berry.Spider.Segmenter.JiebaNet;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;

namespace Berry.Spider.Consumers;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderEventBusRabbitMqModule),
    typeof(SpiderSegmenterJiebaNetModule),
    typeof(SpiderRealTimeAbstractionsModule),
    typeof(SpiderFreeRedisModule),
    typeof(SpiderNaiPanModule),
    typeof(TouTiaoSpiderApplicationModule),
    typeof(SogouSpiderApplicationModule),
    typeof(BaiduSpiderApplicationModule)
)]
public class SpiderConsumersModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<SpiderConsumersModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        //注册服务
        // await context.AddBackgroundWorkerAsync<ServLifetimeCheckerWorker>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //今日头条
        context.Services.AddTransient<ITouTiaoSpider4QuestionEventHandler, TouTiaoSpider4QuestionEventHandler>();
        context.Services.AddTransient<ITouTiaoSpider4QuestionExtNo1EventHandler, TouTiaoSpider4QuestionExtNo1EventHandler>();
        context.Services.AddTransient<ITouTiaoSpider4HighQualityQuestionEventHandler, TouTiaoSpider4HighQualityQuestionEventHandler>();
        context.Services.AddTransient<ITouTiaoSpider4HighQualityQuestionExtNo1EventHandler, TouTiaoSpider4HighQualityQuestionExtNo1EventHandler>();
        context.Services.AddTransient<ITouTiaoSpider4InformationEventHandler, TouTiaoSpider4InformationEventHandler>();
        context.Services.AddTransient<ITouTiaoSpider4InformationCompositionEventHandler, TouTiaoSpider4InformationCompositionEventHandler>();

        //搜狗
        context.Services.AddTransient<ISogouSpider4RelatedSearchEventHandler, SogouSpider4RelatedSearchEventHandler>();
        context.Services.AddTransient<ISogouSpider4WenWenEventHandler, SogouSpider4WenWenEventHandler>();

        //百度
        context.Services.AddTransient<IBaiduSpider4RelatedSearchEventHandler, BaiduSpider4RelatedSearchEventHandler>();
    }
}