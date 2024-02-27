using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.Core;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Berry.Spider.Consumers;

public class SpiderConsumersHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IHostApplicationLifetime _appLifetime;

    public SpiderConsumersHostedService(IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHostApplicationLifetime appLifetime)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
        _appLifetime = appLifetime;

        //注册应用程序启动、停止事件
        ApplicationRegisterHandler();
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

        //初始化
        IBootstrapper? bootstrapper = _abpApplication.ServiceProvider.GetService<IBootstrapper>();
        if (bootstrapper != null)
        {
            await bootstrapper.BootstrapAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }

    private void ApplicationRegisterHandler()
    {
        SpiderOptions? spiderOptions = _configuration.GetSection(nameof(SpiderOptions)).Get<SpiderOptions>();
        if (spiderOptions is not null and { ServLifetimeOptions: { IsEnable: true } })
        {
            _appLifetime.ApplicationStarted.Register(OnRegister);
            _appLifetime.ApplicationStopping.Register(OnUnRegister);
        }
    }

    private void OnRegister()
    {
        ISpiderClientRegister? register = _abpApplication.ServiceProvider.GetService<ISpiderClientRegister>();
        if (register != null)
        {
            AsyncHelper.RunSync(() => register.RegisterAsync());
        }
    }

    private void OnUnRegister()
    {
        ISpiderClientRegister? register = _abpApplication.ServiceProvider.GetService<ISpiderClientRegister>();
        if (register != null)
        {
            AsyncHelper.RunSync(() => register.UnRegisterAsync());
        }
    }
}