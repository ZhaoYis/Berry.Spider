using Berry.Spider.AIGen.Views;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.AIGen;

[DependsOn(typeof(AbpAutofacModule))]
public class SpiderAIGenModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();
    }
}