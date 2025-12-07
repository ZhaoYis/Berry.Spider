using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Agents.AI.Workflows;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;

public sealed class MainWriterFinishedEvent(MainWriterOutput output) : WorkflowEvent(output)
{
    public override string ToString()
    {
        return $"""
                 [生成结果]
                 标题：{output.Title}
                 内容：{output.Content}
                 描述：{output.Description}
                """;
    }
}