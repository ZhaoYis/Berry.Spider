using Berry.Spider.Domain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Berry.Spider.TouTiao.Sender;

public class TouTiaoSpiderSenderHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public TouTiaoSpiderSenderHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<TouTiaoSpiderSenderModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        //TODO:可以通过其他方式来调用
        var touTiaoSpiderService = _abpApplication.ServiceProvider.GetRequiredService<ITouTiaoSpiderAppService>();
        string sources =
            await File.ReadAllTextAsync("/Users/mrzhaoyi/Workspace/DotNetProject/Berry.Spider/doc/0305.txt");
        string[] list = sources.Split("\n");
        foreach (string s in list)
        {
            await touTiaoSpiderService.ExecuteAsync<TouTiaoSpiderRequest>(
                new TouTiaoSpiderRequest() { Keyword = s, SourceFrom = SpiderSourceFrom.TouTiao_Question });
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}