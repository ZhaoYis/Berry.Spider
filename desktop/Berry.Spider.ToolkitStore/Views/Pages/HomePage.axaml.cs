using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages;

public partial class HomePage : UserControlBase
{
    public HomePage()
    {
        HomePageViewModel vm = App.Current.GetRequiredService<HomePageViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}