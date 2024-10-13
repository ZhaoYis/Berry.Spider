using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AutoGen4GroupChat : UserControlBase
{
    public AutoGen4GroupChat()
    {
        AutoGen4GroupChatViewModel vm = App.Current.Services.GetRequiredService<AutoGen4GroupChatViewModel>();
        vm.ShowNotificationMessageEvent += ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}