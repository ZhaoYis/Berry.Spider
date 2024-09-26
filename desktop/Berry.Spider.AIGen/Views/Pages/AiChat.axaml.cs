using Avalonia.Controls;
using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class AiChat : UserControl
{
    public AiChat()
    {
        DataContext = App.Current.Services.GetService<AiChatViewModel>();
        InitializeComponent();
    }
}