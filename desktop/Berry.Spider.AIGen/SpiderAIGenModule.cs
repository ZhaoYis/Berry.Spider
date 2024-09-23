using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using AgileConfig.Client;
using Berry.Spider.AIGen.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.AIGen;

[DependsOn(typeof(AbpAutofacModule))]
public class SpiderAIGenModule : AbpModule
{
    [Experimental("SKEXP0010")]
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var appId = "berry_spider_consumers";
        var secret = "1q2w3E*.com";
        var nodes = "http://124.223.62.114:15000";
        ConfigClient client = new ConfigClient(appId, secret, nodes, "PROD")
        {
            Name = "Berry.Spider.AIGen",
            Tag = ""
        };
        client.ConnectAsync().GetAwaiter().GetResult();

        //注入SK核心Kernel服务
        string modelId = client.Get("OllamaQwenOptions:ModelId");
        string serviceAddr = client.Get("OllamaOptions:ServiceAddr");
        context.Services.AddTransient(serviceProvider =>
        {
            var handler = new OpenAIHttpClientHandler(serviceAddr);
            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId: modelId,
                    endpoint: new Uri(serviceAddr),
                    apiKey: null, httpClient: new HttpClient(handler))
                .Build();
            return kernel;
        });

        context.Services.AddSingleton(client);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();
    }
}