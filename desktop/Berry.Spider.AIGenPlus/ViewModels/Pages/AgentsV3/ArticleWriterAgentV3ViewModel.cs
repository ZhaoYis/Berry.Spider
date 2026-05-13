using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
    public const string ScopeName = "ArticleWriterAgentV3";
    public const string ReviewRoundStateKey = "ReviewRound";
    public const int MaxReviewRounds = 3;

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
        this.AiResponseText = string.Empty;
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");

        try
        {
            string taskId = Guid.NewGuid().ToString("N");
            SummarizeWriterExecutor summarizeWriterExecutor =
                new SummarizeWriterExecutor(nameof(SummarizeWriterExecutor), summarizeWriterAgent, taskId);
            MainWriterExecutor mainWriterExecutor =
                new MainWriterExecutor(nameof(MainWriterExecutor), mainWriterAgent, taskId);
            ReviewerExecutor reviewerExecutor = new ReviewerExecutor(nameof(ReviewerExecutor), reviewerAgent, taskId);
            //构建工作流
            var workflow = new WorkflowBuilder(summarizeWriterExecutor)
                // .AddEdge(source: summarizeWriterExecutor, target: mainWriterExecutor)
                // .AddEdge(source: mainWriterExecutor, target: reviewerExecutor)
                // .AddEdge(source: reviewerExecutor, target: mainWriterExecutor)
                // .WithOutputFrom(reviewerExecutor)
                .Build();

            ChatMessage userMessage = new ChatMessage(ChatRole.User, this.UserInput);
            StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, userMessage);
            var tiggerStat = await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            if (tiggerStat is false)
            {
                this.ShowNotificationMessage("触发Agent执行失败");
            }

            await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync().ConfigureAwait(false))
            {
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
                        string errorMessage = workflowErrorEvent.Data?.ToString() ?? "Workflow error";
                        Debug.WriteLine($"[{nameof(WorkflowErrorEvent)}] {errorMessage}");
                        this.ShowNotificationMessage("执行失败", errorMessage);
                        break;
                    case WorkflowOutputEvent workflowOutputEvent:
                        this.AiResponseText += workflowOutputEvent.Data;
                        break;
                    case ExecutorInvokedEvent executorInvoked:
                        Debug.WriteLine($"EXECUTOR ENTER #{executorInvoked.ExecutorId}");
                        break;
                    case ExecutorCompletedEvent executorCompleted:
                        Debug.WriteLine($"EXECUTOR EXIT #{executorCompleted.ExecutorId}");
                        break;
                    default:
                        Debug.WriteLine($"[{nameof(ArticleWriterAgentV3ViewModel)}] {workflowEvent.ToString()}");
                        break;
                }
            }
        }
        catch (OperationCanceledException e)
        {
            this.ShowNotificationMessage("执行取消", "生成已取消");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[{nameof(ArticleWriterAgentV3ViewModel)}] {e.ToString()}");
            this.ShowNotificationMessage("执行失败", e.Message);
        }
    }

    /// <summary>
    /// 是否可以执行
    /// </summary>
    /// <returns></returns>
    private bool CanExecute() => !string.IsNullOrWhiteSpace(this.UserInput);
}