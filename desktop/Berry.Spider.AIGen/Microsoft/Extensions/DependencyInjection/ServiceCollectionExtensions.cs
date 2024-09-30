using System;
using System.Net.Http;
using AgileConfig.Client;
using Berry.Spider.AIGen;
using Berry.Spider.SemanticKernel.Ollama;
using Berry.Spider.SemanticKernel.Ollama.Qwen;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Sqlite;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注入SK核心Kernel服务
    /// </summary>
    public static void AddKernel(this IServiceCollection services, ConfigClient client)
    {
        string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
        string modelId = client.Get("OllamaQwenOptions:ModelId");
        services.AddTransient<Kernel>(serviceProvider =>
        {
            var handler = new OpenAIHttpClientHandler(serviceAddr);
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(
                modelId: modelId,
                endpoint: new Uri(serviceAddr),
                apiKey: null,
                httpClient: new HttpClient(handler)
            );
            return builder.Build();
        });
    }

    /// <summary>
    /// 注入SK核心ISemanticTextMemory服务
    /// </summary>
    public static void AddSemanticTextMemory(this IServiceCollection services)
    {
        services.AddTransient<ISemanticTextMemory>(serviceProvider =>
        {
            var memoryBuilder = new MemoryBuilder();
            ITextEmbeddingGenerationService tegService = serviceProvider.GetRequiredService<ITextEmbeddingGenerationService>();
            memoryBuilder.WithTextEmbeddingGeneration(tegService);
            IMemoryStore memoryStore = AsyncHelper.RunSync(async () => await SqliteMemoryStore.ConnectAsync("memstore.db"));
            memoryBuilder.WithMemoryStore(memoryStore);
            ISemanticTextMemory textMemory = memoryBuilder.Build();
            return textMemory;
        });
    }

    public static void ConfigureOllamaOptions(this IServiceCollection services, ConfigClient client)
    {
        Check.NotNull(services, nameof(services));
        Check.NotNull(client, nameof(client));

        bool isEnable = bool.Parse(client.Get("OllamaOptions:IsEnable"));
        string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
        services.Configure<OllamaOptions>(opt =>
        {
            opt.IsEnable = isEnable;
            opt.ServiceAddr = serviceAddr;
        });
    }

    public static void ConfigureOllamaQwenOptions(this IServiceCollection services, ConfigClient client)
    {
        Check.NotNull(services, nameof(services));
        Check.NotNull(client, nameof(client));

        string modelId = client.Get("OllamaQwenOptions:ModelId");
        string embeddingModelId = client.Get("OllamaQwenOptions:EmbeddingModelId");
        services.Configure<OllamaQwenOptions>(opt =>
        {
            opt.ModelId = modelId;
            opt.EmbeddingModelId = embeddingModelId;
        });
    }
}