using Microsoft.Extensions.DependencyInjection;
using Polly;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Berry.Spider.HttpApi.StaticClient;

[DependsOn(typeof(AbpHttpClientModule), typeof(SpiderContractsModule))]
public class SpiderHttpApiStaticClientModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpHttpClientBuilderOptions>(options =>
        {
            options.ProxyClientBuildActions.Add((remoteServiceName, clientBuilder) =>
            {
                clientBuilder.AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: i => TimeSpan.FromSeconds(Math.Pow(2, i))
                    )
                );
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Prepare for static client proxy generation
        context.Services.AddStaticHttpClientProxies(
            typeof(SpiderContractsModule).Assembly
        );

        // // Create dynamic client proxies
        // context.Services.AddHttpClientProxies(
        //     typeof(SpiderContractsModule).Assembly
        // );
    }
}