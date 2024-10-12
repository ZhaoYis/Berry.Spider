using System.Text;
using System.Threading.Tasks;
using AutoGen.Core;
using AutoGen.SemanticKernel;
using AutoGen.SemanticKernel.Extension;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AutoGen4SKAgentViewModel(Kernel kernel) : ViewModelRecipientBase, ITransientDependency
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

        // create a semantic kernel agent
        var semanticKernelAgent = new SemanticKernelAgent(
            kernel: kernel,
            name: "assistant",
            systemMessage: "你是一名助手，帮助用户完成一些任务。必要的时候你需要调度一些系统定义好的函数来完成任务。"
        );
        semanticKernelAgent.RegisterMessageConnector();
        semanticKernelAgent.RegisterPrintMessage();
        //semanticKernelAgent.RegisterFunctions();

        // SemanticKernelAgent supports the following message types:
        // - IMessage<ChatMessageContent> where ChatMessageContent is from Azure.AI.OpenAI
        var userMessage = new ChatMessageContent(AuthorRole.User, this.AskAiRequestText);

        // Use MessageEnvelope.Create to create an IMessage<ChatRequestMess>
        var chatMessageContent = MessageEnvelope.Create(userMessage);

        var response = await semanticKernelAgent.SendAsync(chatMessageContent);
        if (response is MessageEnvelope<ChatMessageContent> message)
        {
            this.AskAiResponseText = message.Content.Content ?? "Oops!";
        }

        // StringBuilder response = new StringBuilder();
        // var streamingReply = semanticKernelAgent.GenerateStreamingReplyAsync(new[] { chatMessageContent });
        // await foreach (var streamingMessage in streamingReply)
        // {
        //     if (streamingMessage is MessageEnvelope<StreamingChatMessageContent> message)
        //     {
        //         this.AskAiResponseText = response.Append(message.Content.Content).ToString();
        //     }
        // }

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}