using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI.Responses;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

/// <summary>
/// AI写手采用的Agent为【顺序编排】模式，各Agent定义如下：
/// SummarizeWriterAgent：摘要写手。根据提供的关键词或者句子生成一段语义丰富的摘要信息，以供下一个agent根据摘要信息编写主体内容
/// MainWriterAgent：主要内容写手。根据摘要信息，生成主要内容
/// ReviewerAgent：内容审查员。根据MainWriterAgent输出的内容进行内容审查，并给出一些优化建议
/// </summary>
/// <param name="agentServices">基于Microsoft Agent Framework实现（https://github.com/microsoft/agent-framework）</param>
public partial class ArticleWriterAgentV2ViewModel(
    IEnumerable<IAgentService> agentServices) : ViewModelBase, ITransientDependency
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

        // 顺序执行
        Workflow workflow = AgentWorkflowBuilder.BuildSequential(
            "ArticleWriterWorkflow",
            agentServices.OrderBy(x => x.Order).Select(x => x.GetAgent())
        );

        // 执行工作流
        var input = $"""
                     **主题:** {this.UserInput}

                     **参考资料:**

                     **撰写要求:**
                     - 目标字数: 100
                     - 写作风格: 专业易懂
                     - 目标读者: 中级开发者

                     请按照三个阶段完成博客生成:
                     1. 资料收集: 分析主题和参考资料,提取核心要点
                     2. 博客撰写: 基于收集的资料撰写博客内容
                     3. 质量审查: 评估博客质量,给出评分和建议
                     """;
        var messages = new List<ChatMessage> { new ChatMessage(ChatRole.User, input) };
        await using var run = await InProcessExecution.StreamAsync(workflow, messages);
        // 发送 TurnToken 触发 Agent 执行
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (var workflowEvent in run.WatchStreamAsync())
        {
            // 处理工作流事件（可以根据实际需求添加或者减少处理逻辑）
            switch (workflowEvent)
            {
                case ExecutorInvokedEvent executorInvoked:
                    Debug.WriteLine($"EXECUTOR ENTER #{executorInvoked.ExecutorId}");
                    break;
                case ExecutorCompletedEvent executorCompleted:
                    Debug.WriteLine($"EXECUTOR EXIT #{executorCompleted.ExecutorId}");
                    break;
                case ExecutorFailedEvent executorFailure:
                    Debug.WriteLine(
                        $"STEP ERROR #{executorFailure.ExecutorId}: {executorFailure.Data?.Message ?? "Unknown"}");
                    break;
                case WorkflowErrorEvent workflowError:
                    throw workflowError.Data as Exception ?? new InvalidOperationException("Unexpected failure...");
                case SuperStepCompletedEvent checkpointCompleted:
                    CheckpointInfo? lastCheckpoint = checkpointCompleted.CompletionInfo?.Checkpoint;
                    Debug.WriteLine(
                        $"CHECKPOINT x{checkpointCompleted.StepNumber} [{lastCheckpoint?.CheckpointId ?? "(none)"}]");
                    break;
                case RequestInfoEvent requestInfo:
                    Debug.WriteLine($"REQUEST #{requestInfo.Request.RequestId}");
                    break;
                case AgentRunUpdateEvent streamEvent:
                    Debug.WriteLine($"[{streamEvent.ExecutorId}] 输出：{streamEvent.Update.Text}");

                    // 函数和工具调用的输出
                    ChatResponseUpdate? chatUpdate = streamEvent.Update.RawRepresentation as ChatResponseUpdate;
                    switch (chatUpdate?.RawRepresentation)
                    {
                        case FunctionCallResponseItem actionUpdate:
                            Debug.Write($"Calling function: {actionUpdate.FunctionName} [{actionUpdate.CallId}]");
                            break;
                        case McpToolCallItem actionUpdate:
                            Debug.Write($"Calling tool: {actionUpdate.ToolName} [{actionUpdate.Id}]");
                            break;
                    }

                    // 过滤掉函数调用和工具调用的输出
                    if (chatUpdate?.RawRepresentation is not FunctionCallResponseItem and not McpToolCallItem)
                    {
                        this.AiResponseText += streamEvent.Update.Text;
                    }

                    break;
                case AgentRunResponseEvent messageEvent:
                    if (messageEvent.Response.Usage is not null)
                    {
                        Debug.WriteLine(
                            $"[Tokens Total: {messageEvent.Response.Usage.TotalTokenCount}, Input: {messageEvent.Response.Usage.InputTokenCount}, Output: {messageEvent.Response.Usage.OutputTokenCount}]");
                    }

                    break;
                case WorkflowOutputEvent workflowOutputEvent:
                    this.ShowNotificationMessage("AI创作完成");
                    break;
                default:
                    Debug.WriteLine($"UNHANDLED: {workflowEvent.GetType().Name}");
                    break;
            }
        }
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.UserInput);
    }
}