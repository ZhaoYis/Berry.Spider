using System;
using AgileConfig.Client;
using Berry.Spider.AIGenPlus;
using Microsoft.Extensions.AI;
using Volo.Abp;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddOllamaChatClient(this IServiceCollection services, ConfigClient client)
    {
        Check.NotNull(client, nameof(client));

        string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
        string modelId = client.Get("OllamaQwenOptions:ModelId");
        services.AddChatClient(_ => new ChatClientBuilder(new OllamaChatClient(serviceAddr, modelId))
            .UseFunctionInvocation()
            //.UseDistributedCache()
            //.UseOpenTelemetry()
            //.UseLogging()
            .Build());
    }

    public static void ConfigureOllamaOptions(this IServiceCollection services, ConfigClient client)
    {
        Check.NotNull(client, nameof(client));

        bool isEnable = bool.Parse(client.Get("OllamaOptions:IsEnable"));
        string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
        services.Configure<OllamaOptions>(opt =>
        {
            opt.IsEnable = isEnable;
            opt.ServiceAddr = serviceAddr;
        });
    }
}