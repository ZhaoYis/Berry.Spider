using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;

public sealed class ReviewerExecutor(string id, IReviewerAgent reviewerAgent)
    : ReflectingExecutor<ReviewerExecutor>(id), IMessageHandler<MainWriterOutput, ReviewerOutput>
{
    public async ValueTask<ReviewerOutput> HandleAsync(MainWriterOutput mainWriterOutput, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = $"""
                         请审核以下内容：
                         标题：{mainWriterOutput.Title}
                         描述：{mainWriterOutput.Description}
                         内容：{mainWriterOutput.Content}
                         """;
        string input = reviewerAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await reviewerAgent.ExecuteAsync(input, this.Id);
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