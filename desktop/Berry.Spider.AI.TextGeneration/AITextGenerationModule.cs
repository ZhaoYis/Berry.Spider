using Berry.Spider.AI.TextGeneration.Commands;
using Berry.Spider.Core.Commands;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.NaiPan;
using Berry.Spider.SemanticKernel.Ollama.Qwen2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.AI.TextGeneration;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderNaiPanModule),
    typeof(SpiderSKOllamaQwen2Module)
)]
public class AITextGenerationModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<AITextGenerationModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation("EnvironmentName => {EnvironmentName}", hostEnvironment.EnvironmentName);

        //启动文件监听服务
        context.ServiceProvider.GetRequiredService<FileWatcherService>().Start();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<FileWatcherService>();
        context.Services.RegisterCommand(opt =>
        {
            opt.Commands.Add(nameof(WatcherChangeTypes.Created), typeof(FileOrFolderCreatedCommand));
            opt.Commands.Add(nameof(WatcherChangeTypes.Deleted), typeof(FileOrFolderDeletedCommand));
            opt.Commands.Add(nameof(WatcherChangeTypes.Changed), typeof(FileOrFolderChangedCommand));
            opt.Commands.Add(nameof(WatcherChangeTypes.Renamed), typeof(FileOrFolderRenamedCommand));
        });

        //注入SK核心Kernel服务
        context.Services.AddTransient(serviceProvider =>
        {
            KernelPluginCollection pluginCollection = new KernelPluginCollection();
            return new Kernel(serviceProvider, pluginCollection);
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        //停止文件监听服务
        context.ServiceProvider.GetRequiredService<FileWatcherService>().Stop();
    }
}