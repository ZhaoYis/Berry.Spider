using AgileConfig.Client;
using Berry.Spider.AIGen.Views;
using Berry.Spider.SemanticKernel.Ollama.Qwen;
using Berry.Spider.SemanticKernel.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Berry.Spider.AIGen;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

[DependsOn(typeof(AbpAutofacModule),
    typeof(SpiderSKOllamaQwenModule),
    typeof(SpiderSKPluginModule))]
public class SpiderAIGenModule : AbpModule
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
                context.Services.AddKernel(client);
                context.Services.AddSingleton(client);

                context.Services.ConfigureOllamaOptions(client);
                context.Services.ConfigureOllamaQwenOptions(client);
            }
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSemanticTextMemory();
        context.Services.AddSingleton<MainWindow>();
    }
}