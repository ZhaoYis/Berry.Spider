using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages.Agents;

public partial class ArticleWriterAgentV2 : UserControlBase
{
    public ArticleWriterAgentV2()
    {
        ArticleWriterAgentV2ViewModel vm = App.Current.GetRequiredService<ArticleWriterAgentV2ViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}