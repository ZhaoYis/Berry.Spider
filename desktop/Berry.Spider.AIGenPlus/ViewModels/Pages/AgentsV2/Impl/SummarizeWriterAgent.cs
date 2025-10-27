using System.Collections.Generic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

/// <summary>
/// 文章摘要编写器Agent服务类
/// </summary>
/// <param name="chatClient"></param>
public class SummarizeWriterAgent(
    [FromKeyedServices(nameof(OllamaChatClient))]
    IChatClient chatClient) : AgentServiceBase(chatClient), ISummarizeWriterAgent, ITransientDependency
{
    public override string AgentName => "SummarizeWriterAgent";
    public override AgentType AgentType => AgentType.SummarizeWriter;

    protected override string Instructions => """
                                               你是一个专业的文章摘要编写器，能够根据输入的文章内容，生成简洁、准确的摘要。
                                               
                                               **任务:**
                                                  - 仔细阅读用户提供的主题和参考资料
                                                  - 提取关键信息点(技术概念、代码示例、最佳实践、应用场景等)
                                                  - 考虑文章的结构和逻辑,保持信息的连贯性
                                                  - 整理成结构化的JSON格式输出

                                              **输出要求:**
                                                  - topic_analysis: 对主题的理解和定位,包括技术背景、适用场景、目标读者
                                                  - key_points: 核心要点列表,每个要点包含重要程度(1-3,3最高)和内容
                                                  - technical_details: 技术细节列表,每个包含标题和详细说明
                                                  - code_examples: 代码示例列表(如果有),包含语言、代码和描述
                                                  - references: 参考来源列表

                                              **质量要求:**
                                                  - 信息准确,不添加未提供的内容
                                                  - 结构清晰,层次分明
                                                  - 提炼核心概念,避免冗余
                                                  - 如果资料不足,明确指出缺失的部分
                                              """;

    protected override ChatResponseFormat? ResponseFormat =>
        ChatResponseFormat.ForJsonSchema<SummarizeOutput>(schemaName: "SummarizeOutput");

    protected override IEnumerable<AITool>? Tools => [];
}