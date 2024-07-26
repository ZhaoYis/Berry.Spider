using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Berry.Spider.SemanticKernel.Ollama;

public class SpiderSKOllamaModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
    }
}