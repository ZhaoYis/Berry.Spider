using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels;
using Berry.Spider.AIGenPlus.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;

namespace Berry.Spider.AIGenPlus;

public partial class App : Application
{
    private IAbpApplicationWithInternalServiceProvider? _abpApplication;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public new static App Current => (App)Application.Current!;
    public IServiceProvider Services { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .CreateLogger();

        try
        {
            Log.Information("Starting Avalonia.AIGenPlus host.");

            _abpApplication = AbpApplicationFactory.Create<SpiderAIGenPlusModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            });

            _abpApplication.Initialize();

            this.Services = _abpApplication.Services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                //初始化MainWindow
                MainWindow mainWindow = this.Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = this.Services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = mainWindow;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
        }
        finally
        {
            base.OnFrameworkInitializationCompleted();
        }
    }
}