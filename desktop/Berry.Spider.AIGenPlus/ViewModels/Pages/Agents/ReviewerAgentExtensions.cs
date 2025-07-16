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
                                                    """"
                                                    As an experienced content reviewer, your task is to analyze the provided content and offer optimization suggestions. Your feedback should help enhance clarity, engagement, and overall quality of the content.

                                                    Please follow these guidelines for the content review:
                                                    1. Assess the content's clarity and coherence, ensuring the message is easily understood.
                                                    2. Evaluate the engagement level, suggesting improvements to captivate the target audience.
                                                    3. Check for grammar, style, and tone consistency throughout the content.
                                                    4. Provide actionable suggestions to optimize structure, improve readability, or enrich content with additional examples or data.

                                                    Example Content:
                                                    """
                                                    Artificial Intelligence (AI) is transforming industries by automating complex processes. However, its integration raises ethical questions, and there's a need for clear guidelines to govern its use. The potential of AI to improve efficiency is significant, but it's important to balance technological advancement with ethical considerations.
                                                    """

                                                    Example Suggestions:
                                                    - Improve clarity by defining key terms, such as what constitutes "ethical questions."
                                                    - Increase engagement by incorporating real-world examples of AI automation.
                                                    - Ensure consistency in tone by maintaining an informative but neutral stance throughout.
                                                    - Consider organizing content with headings and bullet points for better readability.

                                                    Now, review the content provided below and offer your suggestions:
                                                    """",
                                                    "");
        ChatCompletionAgent reviewerAgent = kernel.CreateAgent(reviewerAgentCreator);
        return reviewerAgent;
    }
}