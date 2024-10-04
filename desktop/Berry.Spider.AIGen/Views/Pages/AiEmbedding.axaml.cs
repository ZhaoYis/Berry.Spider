using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AiEmbedding : UserControlBase
{
    public AiEmbedding()
    {
        AiEmbeddingViewModel vm = App.Current.Services.GetRequiredService<AiEmbeddingViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        this.InitializeComponent();
    }
}