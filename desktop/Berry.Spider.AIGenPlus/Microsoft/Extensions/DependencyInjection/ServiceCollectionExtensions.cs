using System;
using System.ClientModel;
using AgileConfig.Client;
using Berry.Spider.AIGenPlus;
using Berry.Spider.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAI;
using OpenAI.Embeddings;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    #region OpenAI AI Client

    /// <summary>
    /// 注入OpenAI AI客户端服务
    /// </summary>
    public static void AddOpenAIClient(this IServiceCollection services, IConfiguration configuration)
    {
        Check.NotNull(configuration, nameof(configuration));

        OpenAIOptions? openAIOptions = configuration.GetSection(nameof(OpenAIOptions)).Get<OpenAIOptions>();
        Check.NotNull(openAIOptions, nameof(openAIOptions));

        var apiKeyCredential = new ApiKeyCredential(openAIOptions.ApiKey);
        var aiClientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(openAIOptions.ServiceAddr)
        };

        // 注入OpenAI Chat Client
        var openAiClient = new OpenAIClient(apiKeyCredential, aiClientOptions)
            .GetChatClient(openAIOptions.ModelId)
            .AsIChatClient();
        services.AddKeyedChatClient(nameof(OpenAIClient), _ => new ChatClientBuilder(openAiClient)
            .UseFunctionInvocation()
            .Build());

        // 注入OpenAI Embedding Client
        EmbeddingClient embeddingClient =
            new EmbeddingClient(openAIOptions.EmbeddingModelId, apiKeyCredential, aiClientOptions);
        var embeddingGenerator = embeddingClient.AsIEmbeddingGenerator();
        services.AddKeyedSingleton("OpenAIEmbeddingGenerator", embeddingGenerator);
    }

    #endregion

    /// <summary>
    /// 注入OpenAI Semantic Kernel服务
    /// </summary>
    public static void AddOpenAISKernel(this IServiceCollection services, IConfiguration configuration)
    {
        Check.NotNull(configuration, nameof(configuration));
        OpenAIOptions? openAIOptions = configuration.GetSection(nameof(OpenAIOptions)).Get<OpenAIOptions>();
        Check.NotNull(openAIOptions, nameof(openAIOptions));

        var apiKeyCredential = new ApiKeyCredential(openAIOptions.ApiKey);
        var aiClientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(openAIOptions.ServiceAddr)
        };

        // 注入OpenAI Chat Client
        var openAiClient = new OpenAIClient(apiKeyCredential, aiClientOptions);
        services.AddTransient<Kernel>(sp =>
        {
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(openAIOptions.ModelId, openAiClient)
                .AddOpenAIChatClient(openAIOptions.ModelId, openAiClient);
            //注入自定义插件
            builder.Plugins.AddPlugins();
            return builder.Build();
        });
    }

    #region SKernel(Ollama)

    /// <summary>
    /// 注入Ollama Semantic Kernel服务
    /// </summary>
    public static void AddOllamaSKernel(this IServiceCollection services, IConfiguration configuration)
    {
        OllamaOptions? ollamaOptions = configuration.GetSection(nameof(OllamaOptions)).Get<OllamaOptions>();
        Check.NotNull(ollamaOptions, nameof(ollamaOptions));

        services.AddTransient<Kernel>(serviceProvider =>
        {
            var builder = Kernel.CreateBuilder()
                .AddOllamaChatCompletion(modelId: ollamaOptions.ModelId, endpoint: new Uri(ollamaOptions.ServiceAddr))
                .AddOllamaChatClient(modelId: ollamaOptions.ModelId);
            //注入自定义插件
            builder.Plugins.AddPlugins();
            return builder.Build();
        });
    }

    #endregion

    # region Ollama AI Client

    /// <summary>
    /// 注入Ollama AI客户端服务
    /// </summary>
    public static void AddOllamaAiClient(this IServiceCollection services, IConfiguration configuration)
    {
        Check.NotNull(configuration, nameof(configuration));

        ConfigClientOptions? configClientOptions =
            configuration.GetSection(nameof(ConfigClientOptions)).Get<ConfigClientOptions>();
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
            else
            {
                throw new AbpInitializationException("Ollama Chat Client is not configured.");
            }
        }
        else
        {
            services.AddOllamaChatClient(configuration);
            services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
        }
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
    }

    private static void AddOllamaKeyedEmbeddingClient(this IServiceCollection services, OllamaOptions options)
    {
        //embedding client
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
            new OllamaEmbeddingGenerator(options.ServiceAddr, options.EmbeddingModelId);
        services.AddKeyedSingleton(nameof(OllamaEmbeddingGenerator), embeddingGenerator);
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

    # endregion
}