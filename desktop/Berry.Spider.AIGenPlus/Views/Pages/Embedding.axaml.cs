using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGenPlus.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages;

public partial class Embedding : UserControlBase
{
    public Embedding()
    {
        EmbeddingViewModel vm = App.Current.GetRequiredService<EmbeddingViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();
    }
}