using Berry.Spider.AIGen.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGen.Views.Pages;

public partial class FunctionCalling : UserControlBase
{
    public FunctionCalling()
    {
        FunctionCallingViewModel vm = App.Current.Services.GetRequiredService<FunctionCallingViewModel>();
        vm.ShowNotificationMessageEvent += ShowNotificationMessage;
        this.DataContext = vm;
        this.InitializeComponent();
    }
}