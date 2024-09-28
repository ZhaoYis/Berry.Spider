using System;
using System.Net.Http;
using AgileConfig.Client;
using Berry.Spider.AIGen.Views;
using Berry.Spider.SemanticKernel.Ollama;
using Berry.Spider.SemanticKernel.Ollama.Qwen;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Sqlite;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Berry.Spider.AIGen;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

[DependsOn(typeof(AbpAutofacModule), typeof(SpiderSKOllamaQwenModule))]
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
                //注入SK核心Kernel服务
                bool isEnable = bool.Parse(client.Get("OllamaOptions:IsEnable"));
                string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
                string modelId = client.Get("OllamaQwenOptions:ModelId");
                string embeddingModelId = client.Get("OllamaQwenOptions:EmbeddingModelId");
                context.Services.AddTransient<Kernel>(serviceProvider =>
                {
                    var handler = new OpenAIHttpClientHandler(serviceAddr);
                    var kernel = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(modelId: modelId,
                            endpoint: new Uri(serviceAddr),
                            apiKey: null,
                            httpClient: new HttpClient(handler))
                        .Build();
                    return kernel;
                });

                context.Services.AddSingleton(client);
                context.Services.Configure<OllamaOptions>(opt =>
                {
                    opt.IsEnable = isEnable;
                    opt.ServiceAddr = serviceAddr;
                });
                context.Services.Configure<OllamaQwenOptions>(opt =>
                {
                    opt.ModelId = modelId;
                    opt.EmbeddingModelId = embeddingModelId;
                });
            }
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //注入SK核心ISemanticTextMemory服务
        context.Services.AddTransient<ISemanticTextMemory>(serviceProvider =>
        {
            var memoryBuilder = new MemoryBuilder();
            ITextEmbeddingGenerationService tegService = serviceProvider.GetRequiredService<ITextEmbeddingGenerationService>();
            memoryBuilder.WithTextEmbeddingGeneration(tegService);
            IMemoryStore memoryStore = AsyncHelper.RunSync(async () => await SqliteMemoryStore.ConnectAsync("memstore.db"));
            memoryBuilder.WithMemoryStore(memoryStore);
            ISemanticTextMemory textMemory = memoryBuilder.Build();
            return textMemory;
        });

        context.Services.AddSingleton<MainWindow>();
    }
}