using AgileConfig.Client;
using Berry.Spider.Application;
using Berry.Spider.Baidu;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.FreeRedis;
using Berry.Spider.Segmenter.JiebaNet;
using Berry.Spider.Sogou;
using Berry.Spider.ToolkitStore.ViewModels;
using Berry.Spider.ToolkitStore.Views;
using Berry.Spider.TouTiao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Berry.Spider.ToolkitStore;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderSegmenterJiebaNetModule),
    typeof(SpiderFreeRedisModule),
    typeof(SpiderApplicationModule),
    typeof(TouTiaoSpiderApplicationModule),
    typeof(SogouSpiderApplicationModule),
    typeof(BaiduSpiderApplicationModule))]
public class SpiderToolkitStoreModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<SpiderToolkitStoreModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ConfigClientOptions? configClientOptions = configuration.GetSection(nameof(ConfigClientOptions)).Get<ConfigClientOptions>();
        if (configClientOptions is not null)
        {
            ConfigClient client = new ConfigClient(configClientOptions);
            bool isConnect = AsyncHelper.RunSync(async () => await client.ConnectAsync());
            if (isConnect)
            {
                context.Services.AddSingleton<ConfigClient>(client);
            }
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>(sp => new MainWindow
        {
            DataContext = sp.GetRequiredService<MainWindowViewModel>()
        });
        //注册MediatR
        context.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });
    }
}