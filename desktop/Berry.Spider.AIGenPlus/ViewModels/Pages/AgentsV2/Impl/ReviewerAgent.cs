using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class ReviewerAgent(
    [FromKeyedServices("OpenAIClient")] IChatClient chatClient) : AgentServiceBase(chatClient), IReviewerAgent
{
    public override string AgentName => nameof(ReviewerAgent);

    /// <summary>
    /// Agent执行顺序
    /// </summary>
    public override int Order => 3;

    /// <summary>
    /// 降低温度以提高输出稳定性
    /// </summary>
    protected override float Temperature => 0.3f;

    protected override string Instructions =>
        """
        你是一位严格的技术内容审查专家,根据用户提供的内容（如果有）,负责对其进行全面质量评估。

        **审查标准:**
        1. **准确性(40分)**: 
           - 技术概念定义是否准确
           - 代码示例是否正确可运行
           - 引用数据是否真实可靠
           - 是否包含过时或错误信息
        2. **逻辑性(30分)**:
           - 文章结构是否清晰、层次分明
           - 论证是否充分、有理有据
           - 段落衔接是否自然流畅
           - 是否存在跳跃式思维或逻辑漏洞
        3. **原创性(20分)**:
           - 是否有独特见解和深度分析
           - 是否是简单资料堆砌
           - 案例是否具有实战价值
           - 是否避免空洞套话
        4. **规范性(10分)**:
           - Markdown格式是否规范
           - 代码块是否正确标注语言
           - 中英文标点是否符合规范
           - 专业术语使用是否统一

        **输出格式(严格按照JSON格式,不要添加任何其他文字):**
        ```json
        {
          "overallScore": 85,
          "accuracy": {
            "score": 38,
            "issues": ["问题描述1", "问题描述2"]
          },
          "logic": {
            "score": 25,
            "issues": ["问题描述1"]
          },
          "originality": {
            "score": 18,
            "issues": []
          },
          "formatting": {
            "score": 9,
            "issues": ["问题描述1"]
          },
          "recommendation": "通过",
          "summary": "总体评价和具体修改建议"
        }
        ```

        **评分规则:**
        - 总分 ≥ 80分: recommendation为""通过""
        - 70 ≤ 总分 < 80: recommendation为""需修改""
        - 总分 < 70: recommendation为""不通过""

        **注意事项:**
        - 评分要客观公正,不要过于严苛或宽松
        - issues数组中的每个问题要具体明确,指出位置
        - summary要给出可操作的修改建议
        - 必须严格按照JSON格式输出,不要有多余文字
        """;

    protected override ChatResponseFormat ResponseFormat =>
        ChatResponseFormat.ForJsonSchema<ReviewerOutput>(schemaName: "ReviewerOutput");

    protected override IEnumerable<AITool> Tools => [];

    public override async Task<string> ExecuteAsync(string input, string taskId)
    {
        string response = await base.ExecuteAsync(input, taskId);
        //TODO:判断内容是否符合json标准
        ReviewerOutput? reviewerOutput = this.ParseJsonResponse<ReviewerOutput>(response);
        if (reviewerOutput is null)
        {
            throw new BusinessException($"Agent执行失败，Agent名称：{AgentName}，任务ID：{taskId}，输入：{input}，输出：{response}");
        }

        //TODO:根据解析到的结果可以做一些业务逻辑处理

        Debug.WriteLine(
            $"[{nameof(ReviewerAgent)}]Agent执行成功，Agent名称：{AgentName}，任务ID：{taskId}，输入：{input}，输出：{response}");

        return response;
    }
}