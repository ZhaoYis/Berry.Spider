using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages;

public partial class AiChatViewModel(IChatClient chatClient) : ViewModelBase, ITransientDependency
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

        var streamingResponse = chatClient.CompleteStreamingAsync(this.AskAiRequestText);
        StringBuilder response = new StringBuilder();
        await foreach (var text in streamingResponse)
        {
            this.AskAiResponseText = response.Append(text.Text).ToString();
        }
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}