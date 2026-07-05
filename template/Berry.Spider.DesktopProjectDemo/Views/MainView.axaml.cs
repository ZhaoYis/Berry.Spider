using Avalonia.Controls;
using Berry.Spider.DesktopProjectDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.DesktopProjectDemo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        MainViewModel vm = App.Current.GetRequiredService<MainViewModel>();
        this.DataContext = vm;
        InitializeComponent();
    }
}