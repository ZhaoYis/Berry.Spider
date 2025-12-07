using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Agents.AI.Workflows;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;

public sealed class ReviewerFinishedEvent(ReviewerOutput output) : WorkflowEvent(output)
{
    public override string ToString()
    {
        return $"""
                [审核反馈]

                总评分：{output.OverallScore}
                准确性评分：{output.Accuracy.Score}
                准确性建议：{string.Join(", ", output.Accuracy.Issues)}

                逻辑评分：{output.Logic.Score}
                逻辑建议：{string.Join(", ", output.Logic.Issues)}

                原创性评分：{output.Originality.Score}
                原创性建议：{string.Join(", ", output.Originality.Issues)}

                格式化评分：{output.Formatting.Score}
                格式化建议：{string.Join(", ", output.Formatting.Issues)}

                总建议：{output.Summary}
                推荐：{output.Recommendation}
                """;
    }
}