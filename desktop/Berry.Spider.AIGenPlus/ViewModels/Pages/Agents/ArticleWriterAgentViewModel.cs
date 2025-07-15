using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

/// <summary>
/// https://learn.microsoft.com/zh-cn/semantic-kernel/frameworks/agent/agent-orchestration/sequential?pivots=programming-language-csharp
/// AI写手采用的Agent为【顺序编排】模式，各Agent定义如下：
/// SummarizeWriterAgent：摘要写手。根据提供的关键词或者句子生成一段语义丰富的摘要信息，以供下一个agent根据摘要信息编写主体内容
/// MainWriterAgent：主要内容写手。根据摘要信息，生成主要内容
/// ReviewerAgent：内容审查员。根据MainWriterAgent输出的内容进行内容审查，并给出一些优化建议
/// TerminatorAgent：终结者写手。根据内容优化建议以及主要内容生成最终文章结果
/// </summary>
/// <param name="kernel"></param>
public partial class ArticleWriterAgentViewModel(Kernel kernel) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 用户输入
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GeneratingCommand))]
    private string? _userInput;

    /// <summary>
    /// AI生成的内容
    /// </summary>
    [ObservableProperty] private string? _aiResponseText;

    /// <summary>
    /// 聊天历史记录
    /// </summary>
    private ChatHistory ChatHistory { get; set; } = [];

    private CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <summary>
    /// 开始生成
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task GeneratingAsync()
    {
        Check.NotNullOrWhiteSpace(this.UserInput, nameof(this.UserInput));
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");

        //定义SummarizeWriterAgent
        ChatCompletionAgent summarizeWriterAgent = kernel.CreateSummarizeWriterAgent();
        //定义MainWriterAgent
        ChatCompletionAgent mainWriterAgent = kernel.CreateMainWriterAgent();
        //定义ReviewerAgent
        ChatCompletionAgent reviewerAgent = kernel.CreateReviewerAgent();
        //定义TerminatorAgent
        ChatCompletionAgent terminatorAgent = kernel.CreateerminatorAgent();

        //设置顺序编排器
        InProcessRuntime inProcessRuntime = new InProcessRuntime();
        SequentialOrchestration orchestration = new(summarizeWriterAgent, mainWriterAgent, reviewerAgent, terminatorAgent, terminatorAgent)
        {
            Name = "ArticleWriterAgent",
            ResponseCallback = ResponseCallback
        };
        await inProcessRuntime.StartAsync(this.CancellationTokenSource.Token);

        //开始调度
        var result = await orchestration.InvokeAsync(this.UserInput, inProcessRuntime, this.CancellationTokenSource.Token);
        var output = await result.GetValueAsync(TimeSpan.FromSeconds(60), this.CancellationTokenSource.Token);

        //停止
        await inProcessRuntime.RunUntilIdleAsync();
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.UserInput);
    }

    /// <summary>
    /// 响应内容回调函数
    /// </summary>
    /// <returns></returns>
    private ValueTask ResponseCallback(ChatMessageContent response)
    {
        this.ChatHistory.Add(response);
        return ValueTask.CompletedTask;
    }
}