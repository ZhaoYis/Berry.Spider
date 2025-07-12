using Berry.Spider.AIGenPlus.Views;
using Berry.Spider.SemanticKernel.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.AIGenPlus;

[DependsOn(typeof(AbpAutofacModule),
              typeof(SpiderSKPluginModule))]
public class SpiderAIGenPlusModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddOllamaAiClient(configuration);
        context.Services.AddSKernel(configuration);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();
    }
}