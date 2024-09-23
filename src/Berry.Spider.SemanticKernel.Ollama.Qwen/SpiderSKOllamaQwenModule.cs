using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp.Modularity;

namespace Berry.Spider.SemanticKernel.Ollama.Qwen;

/// <summary>
/// SemanticKernel & Ollama集成qwen2:7b模型
/// 模型地址：https://ollama.com/library/qwen2
/// </summary>
[DependsOn(typeof(SpiderSKOllamaModule))]
public class SpiderSKOllamaQwenModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<OllamaQwenOptions>(configuration.GetSection(nameof(OllamaQwenOptions)));
        
        //TIPS：ollama目前已经兼容了openai的协议，因此如果使用ollama的话可以不用自己实现SK的相关接口
        //参考文章：https://ollama.com/blog/openai-compatibility
        //示例代码：
        // var kernel = Kernel.CreateBuilder()
        //     .AddOpenAIChatCompletion(modelId: "qwen2:7b", apiKey: null, endpoint: new Uri("http://localhost:11434")).Build();
        context.Services.AddSingleton<ITextGenerationService, OllamaQwenTextGenerationService>();
    }
}