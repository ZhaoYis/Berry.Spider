using Berry.Spider.AIGenPlus.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages;

public partial class AiChat : UserControlBase
{
    public AiChat()
    {
        AiChatViewModel vm = App.Current.GetRequiredService<AiChatViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}