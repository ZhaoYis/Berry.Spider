using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages;

public partial class FunctionCall : UserControlBase
{
    public FunctionCall()
    {
        FunctionCallViewModel vm = App.Current.GetRequiredService<FunctionCallViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}