using System.Threading.Tasks;
using AutoGen.Core;
using AutoGen.SemanticKernel;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0110

public partial class AutoGen4SKChatAgentViewModel(Kernel kernel) : ViewModelRecipientBase, ITransientDependency
{
    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(Ask4AgentCommand))]
    private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    /// <summary>
    /// 调度
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task Ask4AgentAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        this.IsActive = true;
        this.Messenger.Send(new NotificationTaskMessage(isRunning: true));

        // The built-in ChatCompletionAgent from semantic kernel.
        var chatAgent = new ChatCompletionAgent()
        {
            Kernel = kernel,
            Name = "assistant",
            Description = "你是一个乐于助人的人工智能助手，帮助用户完成一些任务。"
        };

        var messageConnector = new SemanticKernelChatMessageContentConnector();
        var semanticKernelAgent = new SemanticKernelChatCompletionAgent(chatAgent)
            .RegisterMiddleware(messageConnector) // register message connector so it support AutoGen built-in message types like TextMessage.
            .RegisterPrintMessage(); // pretty print the message to the console

        // SemanticKernelAgent supports the following message types:
        // - IMessage<ChatMessageContent> where ChatMessageContent is from Azure.AI.OpenAI
        var userMessage = new ChatMessageContent(AuthorRole.User, this.AskAiRequestText);

        // Use MessageEnvelope.Create to create an IMessage<ChatRequestMess>
        var chatMessageContent = MessageEnvelope.Create(userMessage);

        var response = await semanticKernelAgent.SendAsync(chatMessageContent);
        if (response is TextMessage message)
        {
            this.AskAiResponseText = message.Content ?? "Oops!";
        }

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}