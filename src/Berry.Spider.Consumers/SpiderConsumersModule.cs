using System.Threading.Tasks;
using Berry.Spider.Baidu;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.EventBus.RabbitMq;
using Berry.Spider.FreeRedis;
using Berry.Spider.Segmenter.JiebaNet;
using Berry.Spider.Sogou;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;

namespace Berry.Spider.Consumers;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderEventBusRabbitMqModule),
    typeof(SpiderSegmenterJiebaNetModule),
    //FreeRedis
    typeof(SpiderFreeRedisModule),
    //今日头条模块
    typeof(TouTiaoSpiderModule),
    //搜狗模块
    typeof(SogouSpiderModule),
    //百度模块
    typeof(BaiduSpiderModule)
)]
public class SpiderConsumersModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<SpiderConsumersModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        //注册服务
        await context.AddBackgroundWorkerAsync<ServLifetimeCheckerWorker>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //今日头条
        context.Services.AddTransient<TouTiaoSpider4QuestionEventHandler>();
        context.Services.AddTransient<TouTiaoSpider4QuestionExtNo1EventHandler>();
        context.Services.AddTransient<TouTiaoSpider4HighQualityQuestionEventHandler>();
        context.Services.AddTransient<TouTiaoSpider4HighQualityQuestionExtNo1EventHandler>();
        context.Services.AddTransient<TouTiaoSpider4InformationEventHandler>();
        context.Services.AddTransient<TouTiaoSpider4InformationCompositionEventHandler>();

        //搜狗
        context.Services.AddTransient<SogouSpider4RelatedSearchEventHandler>();

        //百度
        context.Services.AddTransient<BaiduSpider4RelatedSearchEventHandler>();
    }
}