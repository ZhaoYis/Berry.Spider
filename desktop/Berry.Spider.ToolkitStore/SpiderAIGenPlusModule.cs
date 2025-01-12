using AgileConfig.Client;
using Berry.Spider.AIGenPlus.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Berry.Spider.AIGenPlus;

[DependsOn(typeof(AbpAutofacModule))]
public class SpiderAIGenPlusModule : AbpModule
{
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
                context.Services.AddOllamaChatClient(client);
                context.Services.ConfigureOllamaOptions(client);
            }
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();
    }
}