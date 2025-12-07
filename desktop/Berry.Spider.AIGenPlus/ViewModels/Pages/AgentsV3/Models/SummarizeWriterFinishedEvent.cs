using System.Linq;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Agents.AI.Workflows;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;

public sealed class SummarizeWriterFinishedEvent(SummarizeOutput output) : WorkflowEvent(output)
{
    public override string ToString()
    {
        return $"""
                [摘要创作完成]
                主题分析：{output.TopicAnalysis}

                关键要点：{string.Join("\n", output.KeyPoints.Select(x => $"重要性：{x.Importance} - 内容：{x.Content}"))}

                技术细节：{string.Join("\n", output.TechnicalDetails.Select(x => $"标题：{x.Title} - 描述：{x.Description}"))}

                代码示例：{string.Join("\n", output.CodeExamples.Select(x => $"语言：{x.Language} - 代码：{x.Code}"))}

                引用：{string.Join("\n", output.References.Select(x => $"[{x}]"))}
                """;
    }
}