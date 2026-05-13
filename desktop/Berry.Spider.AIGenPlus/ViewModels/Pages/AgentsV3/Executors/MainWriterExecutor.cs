using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using Microsoft.Agents.AI.Workflows;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;

internal sealed partial class MainWriterExecutor(
    string executorId,
    IMainWriterAgent mainWriterAgent,
    string taskId)
    : Executor(executorId)
{
    /// <summary>
    /// 根据摘要输出创作技术文章
    /// </summary>
    [MessageHandler]
    private async ValueTask<MainWriterOutput> HandlerAsync(SummarizeOutput summarizeOutput, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = $"""
                         请根据以下摘要内容创作一篇技术文章：
                         主题分析：{summarizeOutput.TopicAnalysis}
                         关键要点：{string.Join("\n", summarizeOutput.KeyPoints.Select(x => $"重要性：{x.Importance} - 内容：{x.Content}"))}
                         技术细节：{string.Join("\n", summarizeOutput.TechnicalDetails.Select(x => $"标题：{x.Title} - 描述：{x.Description}"))}
                         代码示例：{string.Join("\n", summarizeOutput.CodeExamples.Select(x => $"语言：{x.Language} - 代码：{x.Code}"))}
                         引用：{string.Join("\n", summarizeOutput.References.Select(x => $"[{x}]"))}
                         """;
        string instructions = mainWriterAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await mainWriterAgent.ExecuteAsync(instructions, taskId);
        MainWriterOutput? mainWriterOutput = JsonSerializer.Deserialize<MainWriterOutput>(result);
        if (mainWriterOutput is null)
        {
            throw new JsonException($"无法将 JSON 字符串反序列化为 {nameof(MainWriterOutput)} 类型。");
        }

        //发布事件
        await context.AddEventAsync(new MainWriterFinishedEvent(mainWriterOutput), cancellationToken);
        return mainWriterOutput;
    }

    /// <summary>
    /// 根据审核输出改进技术文章
    /// </summary>
    [MessageHandler]
    private async ValueTask<MainWriterOutput> HandlerAsync(ReviewerOutput reviewerOutput, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = $"""
                         以下是对你的之前提供的内容审核建议：
                         总评分：{reviewerOutput.OverallScore}
                         准确性评分：{reviewerOutput.Accuracy.Score}
                         准确性建议：{string.Join(", ", reviewerOutput.Accuracy.Issues)}

                         逻辑评分：{reviewerOutput.Logic.Score}
                         逻辑建议：{string.Join(", ", reviewerOutput.Logic.Issues)}

                         原创性评分：{reviewerOutput.Originality.Score}
                         原创性建议：{string.Join(", ", reviewerOutput.Originality.Issues)}

                         格式化评分：{reviewerOutput.Formatting.Score}
                         格式化建议：{string.Join(", ", reviewerOutput.Formatting.Issues)}

                         总建议：{reviewerOutput.Summary}
                         推荐：{reviewerOutput.Recommendation}

                         请根据以上的建议改进你的内容，确保符合要求。
                         """;
        string instructions = mainWriterAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await mainWriterAgent.ExecuteAsync(instructions, taskId);
        MainWriterOutput? mainWriterOutput = JsonSerializer.Deserialize<MainWriterOutput>(result);
        if (mainWriterOutput is null)
        {
            throw new JsonException($"无法将 JSON 字符串反序列化为 {nameof(MainWriterOutput)} 类型。");
        }

        //发布事件
        await context.AddEventAsync(new MainWriterFinishedEvent(mainWriterOutput), cancellationToken);
        return mainWriterOutput;
    }

    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        return protocolBuilder;
    }
}