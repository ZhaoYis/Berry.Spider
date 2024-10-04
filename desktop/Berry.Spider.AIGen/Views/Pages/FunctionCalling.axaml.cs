using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class FunctionCalling : UserControl
{
    public FunctionCalling()
    {
        this.DataContext = App.Current.Services.GetRequiredService<FunctionCallingViewModel>();
        InitializeComponent();
    }
}