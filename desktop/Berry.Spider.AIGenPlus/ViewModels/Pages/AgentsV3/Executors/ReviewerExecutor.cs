using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;

public sealed class ReviewerExecutor(
    string executorId,
    IReviewerAgent reviewerAgent,
    string taskId)
    : ReflectingExecutor<ReviewerExecutor>(executorId), IMessageHandler<MainWriterOutput, ReviewerOutput>
{
    /// <summary>
    /// 根据创作输出，审核技术文章
    /// </summary>
    public async ValueTask<ReviewerOutput> HandleAsync(MainWriterOutput mainWriterOutput, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        //从当前工作流上下文获取用户原始问题
        string? originalQuestion = await context.ReadStateAsync<string>(taskId, cancellationToken);

        string prompt = $"""
                         请审核以下内容：
                         标题：{mainWriterOutput.Title}
                         描述：{mainWriterOutput.Description}
                         内容：{mainWriterOutput.Content}
                         用户原始问题：{originalQuestion ?? "N/A"}
                         """;
        string input = reviewerAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await reviewerAgent.ExecuteAsync(input, taskId);
        ReviewerOutput? reviewerOutput = JsonSerializer.Deserialize<ReviewerOutput>(result);
        if (reviewerOutput is null)
        {
            throw new JsonException($"无法将 JSON 字符串反序列化为 {nameof(ReviewerOutput)} 类型。");
        }

        //发布事件
        await context.AddEventAsync(new ReviewerFinishedEvent(reviewerOutput), cancellationToken);

        //检查审核结果
        if (reviewerOutput.OverallScore >= 80)
        {
            Debug.WriteLine($"审核通过！{mainWriterOutput.Title}");
            await context.YieldOutputAsync($"""
                                            审核通过！
                                            标题：{mainWriterOutput.Title}
                                            内容：{mainWriterOutput.Content}
                                            总评分：{reviewerOutput.OverallScore}分
                                            准确性：{reviewerOutput.Accuracy.Score}分
                                            逻辑性：{reviewerOutput.Logic.Score}分
                                            原创性：{reviewerOutput.Originality.Score}分
                                            格式化：{reviewerOutput.Formatting.Score}分
                                            推荐：{reviewerOutput.Recommendation}
                                            总结：{reviewerOutput.Summary}
                                            """, cancellationToken);
            return reviewerOutput;
        }

        //继续处理
        await context.SendMessageAsync(reviewerOutput, cancellationToken);
        return reviewerOutput;
    }
}