using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AiChat : UserControlBase
{
    public AiChat()
    {
        AiChatViewModel vm = App.Current.Services.GetRequiredService<AiChatViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        this.InitializeComponent();
    }
}