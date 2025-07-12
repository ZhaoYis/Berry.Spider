using System;
using System.Net.Http;
using AgileConfig.Client;
using Berry.Spider.AIGenPlus;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddOllamaAiClient(this IServiceCollection services, IConfiguration configuration)
    {
        Check.NotNull(configuration, nameof(configuration));

        ConfigClientOptions? configClientOptions = configuration.GetSection(nameof(ConfigClientOptions)).Get<ConfigClientOptions>();
        if (configClientOptions is not null)
        {
            ConfigClient client = new ConfigClient(configClientOptions);
            bool isConnect = AsyncHelper.RunSync(async () => await client.ConnectAsync());
            if (isConnect)
            {
                services.AddSingleton<ConfigClient>(client);
                services.AddOllamaChatClient(client);
                services.ConfigureOllamaOptions(client);
            }
        }
        else
        {
            services.AddOllamaChatClient(configuration);
            services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
        }
    }

    /// <summary>
    /// 注入SK核心Kernel服务
    /// </summary>
    public static void AddSKernel(this IServiceCollection services, IConfiguration configuration)
    {
        OllamaOptions? ollamaOptions = configuration.GetSection(nameof(OllamaOptions)).Get<OllamaOptions>();
        Check.NotNull(ollamaOptions, nameof(ollamaOptions));

        services.AddTransient<Kernel>(serviceProvider =>
        {
            var builder = Kernel.CreateBuilder()
                                .AddOllamaChatCompletion(modelId: ollamaOptions.ModelId,
                                                         endpoint: new Uri(ollamaOptions.ServiceAddr))
                                .AddOllamaChatClient(modelId: ollamaOptions.ModelId)
                ;
            //注入自定义插件
            builder.Plugins.AddPlugins();
            return builder.Build();
        });
    }

    private static void AddOllamaChatClient(this IServiceCollection services, IConfiguration configuration)
    {
        OllamaOptions? ollamaOptions = configuration.GetSection(nameof(OllamaOptions)).Get<OllamaOptions>();
        Check.NotNull(ollamaOptions, nameof(ollamaOptions));

        services.AddOllamaKeyedChatClient(ollamaOptions);
        services.AddOllamaKeyedEmbeddingClient(ollamaOptions);
    }

    private static void AddOllamaChatClient(this IServiceCollection services, ConfigClient client)
    {
        OllamaOptions ollamaOptions = BuildOllamaOptions(client);
        Check.NotNull(ollamaOptions, nameof(ollamaOptions));

        services.AddOllamaKeyedChatClient(ollamaOptions);
        services.AddOllamaKeyedEmbeddingClient(ollamaOptions);
    }

    private static void AddOllamaKeyedChatClient(this IServiceCollection services, OllamaOptions options)
    {
        //chat client
        var ollamaChatClient = new OllamaChatClient(options.ServiceAddr, options.ModelId);
        services.AddKeyedChatClient(nameof(OllamaChatClient), _ => new ChatClientBuilder(ollamaChatClient)
                                                                   .UseFunctionInvocation()
                                                                   //.UseDistributedCache()
                                                                   //.UseOpenTelemetry()
                                                                   //.UseLogging()
                                                                   .Build());

        //或者使用OpenAIClient进行初始化
        // var apiKeyCredential = new ApiKeyCredential(options.ModelId);
        // var aiClientOptions = new OpenAIClientOptions
        // {
        //     Endpoint = new Uri(options.ServiceAddr)
        // };
        //
        // var openAiClient = new OpenAIClient(apiKeyCredential, aiClientOptions).AsChatClient(options.ModelId);
        // services.AddKeyedChatClient(serviceKey, _ => new ChatClientBuilder(openAiClient)
        //     .UseFunctionInvocation()
        //     .Build());
    }

    private static void AddOllamaKeyedEmbeddingClient(this IServiceCollection services, OllamaOptions options)
    {
        //embedding client
        services.AddKeyedSingleton(nameof(OllamaEmbeddingGenerator), new OllamaEmbeddingGenerator(options.ServiceAddr, options.EmbeddingModelId));
    }

    private static void ConfigureOllamaOptions(this IServiceCollection services, ConfigClient client)
    {
        services.Configure<OllamaOptions>(opt =>
        {
            OllamaOptions options = BuildOllamaOptions(client);
            opt.IsEnable = options.IsEnable;
            opt.ServiceAddr = options.ServiceAddr;
            opt.ModelId = options.ModelId;
            opt.EmbeddingModelId = options.EmbeddingModelId;
        });
    }

    private static OllamaOptions BuildOllamaOptions(ConfigClient client)
    {
        Check.NotNull(client, nameof(client));

        bool isEnable = bool.Parse(client.Get(nameof(OllamaOptions.IsEnable)));
        string serviceAddr = client.Get(nameof(OllamaOptions.ServiceAddr));
        string modelId = client.Get(nameof(OllamaOptions.ModelId));
        string embeddingModelId = client.Get(nameof(OllamaOptions.EmbeddingModelId));
        return new OllamaOptions
        {
            IsEnable = isEnable,
            ServiceAddr = serviceAddr,
            ModelId = modelId,
            EmbeddingModelId = embeddingModelId
        };
    }
}