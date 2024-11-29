using System.Text;
using System.Threading.Tasks;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

public partial class AiChatViewModel(Kernel kernel, ISemanticTextMemory textMemory) : ViewModelRecipientBase, ITransientDependency
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
    /// 是否启用聊天
    /// </summary>
    [ObservableProperty] private bool _isEnableChat = false;

    /// <summary>
    /// 问AI
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task AskAiAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        this.IsActive = true;
        this.Messenger.Send(new NotificationTaskMessage(isRunning: true));

        if (this.IsEnableChat)
        {
            IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chatHistory = [];
            chatHistory.AddSystemMessage("You are a helpful assistant.");
            chatHistory.AddUserMessage(this.AskAiRequestText);
            StringBuilder response = new StringBuilder();
            await foreach (var text in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                this.AskAiResponseText = response.Append(text.Content).ToString();
            }

            chatHistory.AddSystemMessage(this.AskAiResponseText);
        }
        else
        {
            ITextGenerationService textGenerationService = kernel.GetRequiredService<ITextGenerationService>();
            StringBuilder response = new StringBuilder();
            await foreach (var text in textGenerationService.GetStreamingTextContentsAsync(this.AskAiRequestText))
            {
                this.AskAiResponseText = response.Append(text.Text).ToString();
            }
        }

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}