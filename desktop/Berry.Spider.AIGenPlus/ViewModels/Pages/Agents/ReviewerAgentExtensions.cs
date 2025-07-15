using Berry.Spider.AIGenPlus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

public static class ReviewerAgentExtensions
{
    /// <summary>
    /// ReviewerAgent：内容审查员。根据MainWriterAgent输出的内容进行内容审查，并给出一些优化建议
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static ChatCompletionAgent CreateReviewerAgent(this Kernel kernel)
    {
        ChatAgentCreator reviewerAgentCreator = new("ReviewerAgent",
                                                    """

                                                    """,
                                                    "");
        ChatCompletionAgent reviewerAgent = kernel.CreateAgent(reviewerAgentCreator);
        return reviewerAgent;
    }
}