using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3;

public partial class ArticleWriterAgentV3ViewModel(
    ISummarizeWriterAgent summarizeWriterAgent,
    IMainWriterAgent mainWriterAgent,
    IReviewerAgent reviewerAgent) : ViewModelBase, ITransientDependency
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
    /// 开始生成
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task GeneratingAsync()
    {
        Check.NotNullOrWhiteSpace(this.UserInput, nameof(UserInput));
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");

        string taskId = Guid.NewGuid().ToString("N");
        SummarizeWriterExecutor summarizeWriterExecutor =
            new SummarizeWriterExecutor(nameof(SummarizeWriterExecutor), summarizeWriterAgent, taskId);
        MainWriterExecutor mainWriterExecutor =
            new MainWriterExecutor(nameof(MainWriterExecutor), mainWriterAgent, taskId);
        ReviewerExecutor reviewerExecutor = new ReviewerExecutor(nameof(ReviewerExecutor), reviewerAgent, taskId);
        //构建工作流
        var workflow = new WorkflowBuilder(summarizeWriterExecutor)
            .AddEdge(source: summarizeWriterExecutor, target: mainWriterExecutor)
            .AddEdge(source: mainWriterExecutor, target: reviewerExecutor)
            .AddEdge(source: reviewerExecutor, target: mainWriterExecutor)
            .WithOutputFrom(reviewerExecutor)
            .Build();

        // 工作流输入类型为 string（SummarizeWriterExecutor : IMessageHandler<string, SummarizeOutput>）
        // 因此这里直接传递用户输入字符串，而不是 List<ChatMessage>
        await using StreamingRun run = await InProcessExecution.StreamAsync(workflow, this.UserInput);
        // // 发送 TurnToken 用以触发 Agent 执行
        // var tiggerStat = await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        // if (tiggerStat is false)
        // {
        //     this.ShowNotificationMessage("触发Agent执行失败");
        // }

        int eventCount = 0;

        try
        {
            await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync())
            {
                eventCount++;
                switch (workflowEvent)
                {
                    case SummarizeWriterFinishedEvent summarizeWriterFinishedEvent:
                        Debug.WriteLine(
                            $"[{nameof(SummarizeWriterFinishedEvent)}] {summarizeWriterFinishedEvent.ToString()}");
                        break;
                    case MainWriterFinishedEvent mainWriterFinishedEvent:
                        Debug.WriteLine($"[{nameof(MainWriterFinishedEvent)}] {mainWriterFinishedEvent.ToString()}");
                        break;
                    case ReviewerFinishedEvent reviewerFinishedEvent:
                        Debug.WriteLine($"[{nameof(ReviewerFinishedEvent)}] {reviewerFinishedEvent.ToString()}");
                        break;
                    case WorkflowErrorEvent workflowErrorEvent:
                        Debug.WriteLine(
                            $"[{nameof(WorkflowErrorEvent)}] {workflowErrorEvent.Data?.ToString() ?? "Workflow error"}");
                        break;
                    case WorkflowOutputEvent workflowOutputEvent:
                        this.AiResponseText += workflowOutputEvent.Data;
                        break;
                    default:
                        Debug.WriteLine($"[{nameof(ArticleWriterAgentV3ViewModel)}] {workflowEvent.ToString()}");
                        break;
                }
            }

            if (eventCount == 0)
            {
                Debug.WriteLine(
                    $"[{nameof(ArticleWriterAgentV3ViewModel)}] 未收到任何 WorkflowEvent，可能存在输入类型不匹配或配置问题。");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[{nameof(ArticleWriterAgentV3ViewModel)}] {e.ToString()}");
        }
    }

    /// <summary>
    /// 是否可以执行
    /// </summary>
    /// <returns></returns>
    private bool CanExecute() => !string.IsNullOrWhiteSpace(this.UserInput);
}