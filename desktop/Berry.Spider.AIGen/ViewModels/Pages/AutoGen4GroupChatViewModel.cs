using System.Text;
using System.Threading.Tasks;
using AutoGen;
using AutoGen.Core;
using AutoGen.SemanticKernel;
using AutoGen.SemanticKernel.Extension;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AutoGen4GroupChatViewModel(Kernel kernel) : ViewModelRecipientBase, ITransientDependency
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

        var bingSearchAgent = new SemanticKernelAgent(
                kernel: kernel,
                name: "bing-search",
                systemMessage: """
                               You search results from Bing and return it as-is.
                               You put the original search result between ```bing and ```

                               e.g.
                               ```bing
                               xxx
                               ```
                               """)
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var summarizerAgent = new SemanticKernelAgent(
                kernel: kernel,
                name: "summarizer",
                systemMessage: """
                               You summarize search result from bing in a short and concise manner
                               """)
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var userProxyAgent = new UserProxyAgent(
                name: "user",
                humanInputMode: HumanInputMode.ALWAYS)
            .RegisterPrintMessage();

        var groupChat = new RoundRobinGroupChat(agents: [userProxyAgent, bingSearchAgent, summarizerAgent]);
        var groupChatAgent = new GroupChatManager(groupChat);
        var chatResponse = await userProxyAgent.InitiateChatAsync(
            receiver: groupChatAgent,
            message: "How to deploy an openai resource on azure",
            maxRound: 10);

        StringBuilder response = new StringBuilder();
        foreach (var chat in chatResponse)
        {
            if (chat is TextMessage message)
            {
                this.AskAiResponseText = response.Append(message.Content).ToString();
            }
        }

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}