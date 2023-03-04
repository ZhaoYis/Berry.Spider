using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.Consumers.HttpApi;
using Volo.Abp;

namespace Berry.Spider.Consumers;

public class SpiderConsumersHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public SpiderConsumersHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<SpiderConsumersModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });
        await _abpApplication.InitializeAsync();

        //启动api服务
        await Task.Factory.StartNew(async () =>
        {
            var apiService = _abpApplication.ServiceProvider.GetService<ISpiderConsumerHttpApiService>();
            if (apiService is { })
            {
                await apiService.InitAsync();
            }
        }, cancellationToken);

        //初始化
        IBootstrapper? bootstrapper = _abpApplication.ServiceProvider.GetService<IBootstrapper>();
        if (bootstrapper is { })
        {
            await bootstrapper.BootstrapAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}