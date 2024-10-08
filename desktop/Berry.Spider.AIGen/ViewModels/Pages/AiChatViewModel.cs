using System.Text;
using System.Threading.Tasks;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AiChatViewModel(Kernel kernel) : ViewModelRecipientBase, ITransientDependency
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

        this.IsActive = true;
        this.Messenger.Send(new NotificationTaskMessage(isRunning: true));

        ITextGenerationService textGenerationService = kernel.GetRequiredService<ITextGenerationService>();
        StringBuilder response = new StringBuilder();
        await foreach (var text in textGenerationService.GetStreamingTextContentsAsync(this.AskAiRequestText))
        {
            this.AskAiResponseText = response.Append(text.Text).ToString();
        }

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}