using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages.Agents;

public partial class ArticleWriterAgentV3 : UserControlBase
{
    public ArticleWriterAgentV3()
    {
        ArticleWriterAgentV3ViewModel vm = App.Current.GetRequiredService<ArticleWriterAgentV3ViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}