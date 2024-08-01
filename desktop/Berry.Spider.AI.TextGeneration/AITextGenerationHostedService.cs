using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace Berry.Spider.AI.TextGeneration;

public class AITextGenerationHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment) : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<AITextGenerationModule>(options =>
        {
            options.Services.ReplaceConfiguration(configuration);
            options.Services.AddSingleton(hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}