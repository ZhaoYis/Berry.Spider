using CommunityToolkit.Mvvm.ComponentModel;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 应用名称
    /// </summary>
    [ObservableProperty] private string _applicationTitle = "Berry.Spider.AIGen";

    public MainWindowViewModel()
    {
    }
}