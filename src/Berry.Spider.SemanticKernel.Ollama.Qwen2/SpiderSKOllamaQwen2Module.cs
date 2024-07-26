using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp.Modularity;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen2;

/// <summary>
/// SemanticKernel & Ollama集成qwen2:7b模型
/// 模型地址：https://ollama.com/library/qwen2
/// </summary>
public class SpiderSKOllamaQwen2Module : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ITextGenerationService, OllamaQwen2TextGenerationService>();
    }
}