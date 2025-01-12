using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.ToolkitStore.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.ToolkitStore.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        MainViewModel vm = App.Current.GetRequiredService<MainViewModel>();
        this.DataContext = vm;
        InitializeComponent();
    }
}