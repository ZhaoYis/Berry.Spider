using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages;

public partial class AiGroupChat : UserControlBase
{
    public AiGroupChat()
    {
        AiGroupChatViewModel vm = App.Current.GetRequiredService<AiGroupChatViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}