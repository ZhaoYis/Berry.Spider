using Berry.Spider.DesktopProjectDemo.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.DesktopProjectDemo.Views.Pages;

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