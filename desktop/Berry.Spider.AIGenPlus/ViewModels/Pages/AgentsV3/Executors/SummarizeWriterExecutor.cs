using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using Microsoft.Agents.AI.Workflows;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;

public sealed partial class SummarizeWriterExecutor(
    string executorId,
    ISummarizeWriterAgent summarizeWriterAgent,
    string taskId)
    : Executor(executorId)
{
    /// <summary>
    /// 处理用户原始问题，生成文章摘要
    /// </summary>
    [MessageHandler]
    public async ValueTask<SummarizeOutput> HandleAsync(string message, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = $"""
                         请根据以下任务要求生成文章摘要：
                         {message}
                         """;
        string instructions = summarizeWriterAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await summarizeWriterAgent.ExecuteAsync(instructions, taskId);
        SummarizeOutput? summarizeOutput = JsonSerializer.Deserialize<SummarizeOutput>(result);
        if (summarizeOutput is null)
        {
            throw new JsonException($"无法将 JSON 字符串反序列化为 {nameof(SummarizeOutput)} 类型。");
        }

        //将用户原始问题写入当前工作流上下文,后续步骤可以从上下文获取用户原始问题
        await context.QueueStateUpdateAsync(taskId, message, scopeName: ArticleWriterAgentV3ViewModel.ScopeName,
            cancellationToken: cancellationToken);

        //发布事件
        await context.AddEventAsync(new SummarizeWriterFinishedEvent(summarizeOutput), cancellationToken);
        return summarizeOutput;
    }
}