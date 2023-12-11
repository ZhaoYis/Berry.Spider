using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Volo.Abp.Modularity;

namespace Berry.Spider.Weixin.Work;

[DependsOn(typeof(SpiderWeixinModule))]
public class SpiderWeixinWorkModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        WeixinWorkRobotOptions? robotOptions = configuration.GetSection(nameof(WeixinWorkRobotOptions)).Get<WeixinWorkRobotOptions>();
        if (robotOptions is { })
        {
            context.Services.AddRefitClient<IWeixinWorkRobotClient>().ConfigureHttpClient(configureClient =>
            {
                string baseAddr = robotOptions.BaseAddress;
                configureClient.BaseAddress = new Uri(baseAddr);
            });
        }

        context.Services.Configure<WeixinWorkRobotOptions>(configuration.GetSection(nameof(WeixinWorkRobotOptions)));
    }
}