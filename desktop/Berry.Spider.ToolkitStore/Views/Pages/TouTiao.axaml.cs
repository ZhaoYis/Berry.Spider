using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.ToolkitStore.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.ToolkitStore.Views.Pages;

public partial class TouTiao : UserControlBase
{
    public TouTiao()
    {
        TouTiaoViewModel vm = App.Current.GetRequiredService<TouTiaoViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}