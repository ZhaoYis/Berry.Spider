using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using AgileConfig.Client;
using Berry.Spider.AIGen.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Berry.Spider.AIGen;

[DependsOn(typeof(AbpAutofacModule))]
public class SpiderAIGenModule : AbpModule
{
    [Experimental("SKEXP0010")]
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
                string modelId = client.Get("OllamaQwenOptions:ModelId");
                string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
                context.Services.AddTransient(serviceProvider =>
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
            }
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddSingleton<MainWindow>();
    }
}