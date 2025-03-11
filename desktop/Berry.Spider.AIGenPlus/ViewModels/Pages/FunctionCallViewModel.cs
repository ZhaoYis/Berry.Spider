using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.Functions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages;

public partial class FunctionCallViewModel(
    [FromKeyedServices(nameof(OllamaChatClient))]
    IChatClient chatClient) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AskAiCommand))]
    private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    /// <summary>
    /// 问AI
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task AskAiAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");

        var chatOptons = new ChatOptions
        {
            Tools =
            [
                App.Current.GetRequiredService<DateTimeFunction>()
            ]
        };
        var res = await chatClient.CompleteAsync(this.AskAiRequestText, chatOptons);
        this.AskAiResponseText = res.ToString();
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}