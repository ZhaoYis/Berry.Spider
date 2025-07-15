using Berry.Spider.AIGenPlus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

public static class TerminatorAgentExtensions
{
    /// <summary>
    /// TerminatorAgent：终结者写手。根据内容优化建议以及主要内容生成最终文章结果
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static ChatCompletionAgent CreateerminatorAgent(this Kernel kernel)
    {
        ChatAgentCreator terminatorAgentCreator = new("TerminatorAgent",
                                                      """

                                                      """,
                                                      "");
        ChatCompletionAgent terminatorAgent = kernel.CreateAgent(terminatorAgentCreator);
        return terminatorAgent;
    }
}