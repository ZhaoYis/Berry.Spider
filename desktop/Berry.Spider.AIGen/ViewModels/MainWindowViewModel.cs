using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ITransientDependency
{
    private readonly HelloWorldService _helloWorldService;

    public MainWindowViewModel(HelloWorldService helloWorldService)
    {
        _helloWorldService = helloWorldService;
    }

    public string Greeting => _helloWorldService.SayHello();
}