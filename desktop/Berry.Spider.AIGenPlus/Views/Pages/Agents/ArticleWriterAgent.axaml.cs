using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages.Agents;

public partial class ArticleWriterAgent : UserControlBase
{
    public ArticleWriterAgent()
    {
        ArticleWriterAgentViewModel vm = App.Current.GetRequiredService<ArticleWriterAgentViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}