using System.Threading.Tasks;
using AgileConfig.Client;
using Berry.Spider.AIGen.Models;
using Berry.Spider.SemanticKernel.Shared.FunctionCallers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class FunctionCallingViewModel(ConfigClient configClient, Kernel kernel) : ViewModelRecipientBase, ITransientDependency
{
    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(FunctionCallingCommand))]
    private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    /// <summary>
    /// 函数调度
    /// </summary>
    [RelayCommand(CanExecute = nameof(FunctionCallingCanExecute))]
    private async Task FunctionCallingAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        this.IsActive = true;
        this.Messenger.Send(new NotifyTaskExecuteMessage(isRunning: true));

        UniversalFunctionCaller planner = new(kernel);
        string? result = await planner.RunAsync(this.AskAiRequestText);

        this.Messenger.Send(new NotifyTaskExecuteMessage(isRunning: false));
        this.AskAiResponseText = result!;
    }

    private bool FunctionCallingCanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}