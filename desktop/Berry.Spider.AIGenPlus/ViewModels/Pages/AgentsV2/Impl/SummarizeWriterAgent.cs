using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class SummarizeWriterAgent(
    [FromKeyedServices("OpenAIClient")] IChatClient chatClient) : AgentServiceBase(chatClient), ISummarizeWriterAgent
{
    public override string AgentName => nameof(SummarizeWriterAgent);

    /// <summary>
    /// Agent执行顺序
    /// </summary>
    public override int Order => 1;

    protected override float Temperature => 0.8f;

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

    protected override ChatResponseFormat ResponseFormat =>
        ChatResponseFormat.ForJsonSchema<SummarizeOutput>(schemaName: "SummarizeOutput");

    protected override IEnumerable<AITool> Tools => [];

    [Experimental("MEAI001")]
    public override async Task<string> ExecuteAsync(string input, string taskId)
    {
        string response = await base.ExecuteAsync(input, taskId);
        //TODO:判断内容是否符合json标准
        SummarizeOutput? summarizeOutput = this.ParseJsonResponse<SummarizeOutput>(response);
        if (summarizeOutput is null)
        {
            throw new BusinessException($"Agent执行失败，Agent名称：{AgentName}，任务ID：{taskId}，输入：{input}，输出：{response}");
        }

        //TODO:根据解析到的结果可以做一些业务逻辑处理，例如保存到数据库等

        Debug.WriteLine(
            $"[{nameof(SummarizeWriterAgent)}]Agent执行成功，Agent名称：{AgentName}，任务ID：{taskId}，输入：{input}，输出：{response}");

        return response;
    }
}