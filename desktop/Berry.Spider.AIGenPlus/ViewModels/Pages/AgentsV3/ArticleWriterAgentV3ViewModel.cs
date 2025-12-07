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

        SummarizeWriterExecutor summarizeWriterExecutor =
            new SummarizeWriterExecutor(nameof(SummarizeWriterExecutor), summarizeWriterAgent);
        MainWriterExecutor mainWriterExecutor = new MainWriterExecutor(nameof(MainWriterExecutor), mainWriterAgent);
        ReviewerExecutor reviewerExecutor = new ReviewerExecutor(nameof(ReviewerExecutor), reviewerAgent);
        //构建工作流
        var workflow = new WorkflowBuilder(summarizeWriterExecutor)
            .AddEdge(source: summarizeWriterExecutor, target: mainWriterExecutor)
            .AddEdge(source: mainWriterExecutor, target: reviewerExecutor)
            .AddEdge(source: reviewerExecutor, target: mainWriterExecutor)
            .WithOutputFrom(reviewerExecutor).Build();

        var messages = new List<ChatMessage> { new ChatMessage(ChatRole.User, this.UserInput) };
        await using var run = await InProcessExecution.StreamAsync(workflow, messages);

        try
        {
            await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync())
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
                    case WorkflowOutputEvent workflowOutputEvent:
                        this.AiResponseText += workflowOutputEvent.Data;
                        break;
                    default:
                        Debug.WriteLine($"[{nameof(ArticleWriterAgentV3ViewModel)}] {workflowEvent.ToString()}");
                        break;
                }
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