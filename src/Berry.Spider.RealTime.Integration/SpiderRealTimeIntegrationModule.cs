using Microsoft.Extensions.DependencyInjection;
using Refit;
using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

[DependsOn(typeof(SpiderRealTimeSharedModule))]
public class SpiderRealTimeIntegrationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        string baseAddr = configuration["RemoteServices:BerrySpiderHttpApi:BaseUrl"] ?? throw new ArgumentNullException("RemoteServices BaseUrl is Empty!");

        context.Services.AddRefitClient<IServAgentIntegration>().ConfigureHttpClient(configureClient =>
        {
            //BaseAddress
            configureClient.BaseAddress = new Uri(baseAddr);
        });
    }
}