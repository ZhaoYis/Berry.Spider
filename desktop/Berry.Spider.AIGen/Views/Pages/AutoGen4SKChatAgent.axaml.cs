using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AutoGen4SKChatAgent : UserControlBase
{
    public AutoGen4SKChatAgent()
    {
        AutoGen4SKChatAgentViewModel vm = App.Current.Services.GetRequiredService<AutoGen4SKChatAgentViewModel>();
        vm.ShowNotificationMessageEvent += ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}