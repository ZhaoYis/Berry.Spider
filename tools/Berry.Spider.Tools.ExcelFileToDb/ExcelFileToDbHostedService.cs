﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace Berry.Spider.Tools.ExcelFileToDb;

public class ExcelFileToDbHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public ExcelFileToDbHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<ExcelFileToDbModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        //启动服务
        //await _abpApplication.ServiceProvider.GetRequiredService<IExcelFileToDbAppService>().RunAsync();
        // await _abpApplication.ServiceProvider.GetRequiredService<IExcelFileToDbAppService>().ExportToExcelAsync();
        await _abpApplication.ServiceProvider.GetRequiredService<IExcelFileToDbAppService>().CleanAndExportToExcelAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}