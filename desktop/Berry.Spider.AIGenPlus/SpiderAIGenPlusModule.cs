using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
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
        //run ollama ai client
        //context.Services.AddOllamaAiClient(configuration);
        //add semantic kernel(ollama)
        //context.Services.AddOllamaSKernel(configuration);

        //run openai ai client
        context.Services.AddOpenAIClient(configuration);
        //add semantic kernel(openai)
        context.Services.AddOpenAISKernel(configuration);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();

        //add agent services
        context.Services.AddTransient<IAgentService, SummarizeWriterAgent>();
        context.Services.AddTransient<IAgentService, MainWriterAgent>();
        context.Services.AddTransient<IAgentService, ReviewerAgent>();
    }
}