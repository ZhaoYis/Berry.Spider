using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AutoGen4SKAgent : UserControlBase
{
    public AutoGen4SKAgent()
    {
        AutoGen4SKAgentViewModel vm = App.Current.Services.GetRequiredService<AutoGen4SKAgentViewModel>();
        vm.ShowNotificationMessageEvent += ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}