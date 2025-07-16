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
                                                      """"
                                                      As a professional content creator, your task is to integrate optimization suggestions into the main content provided to produce a high-quality final article. This article should be polished, engaging, and ready for publication.

                                                      Follow these steps to accomplish the task:
                                                      Step 1: Review the optimization suggestions and understand how each can enhance the main content.
                                                      Step 2: Revise the main content by incorporating the suggestions, focusing on improving clarity, engagement, structure, and style.
                                                      Step 3: Ensure the final article maintains consistency in tone and adheres to any specified guidelines or style requirements.
                                                      Step 4: Perform a final proofread to eliminate any grammatical errors and optimize readability.

                                                      Example of Optimization Suggestions:
                                                      """
                                                      - Add more real-world examples to illustrate key points.
                                                      - Use subheadings to break up sections for easier reading.
                                                      - Refine the introduction to better engage the audience.
                                                      """

                                                      Example of Main Content:
                                                      """
                                                      Artificial Intelligence (AI) is increasingly recognized as a vital tool in combating Climate Change. Its capacity to process data and anticipate environmental changes is transforming sustainability efforts.
                                                      """

                                                      Based on the suggestions and main content provided below, generate the final article:
                                                      Suggestions:
                                                      """
                                                      ... insert optimization suggestions here ...
                                                      """

                                                      Main Content:
                                                      """
                                                      ... insert main content here ...
                                                      """
                                                      """",
                                                      "As a professional content creator.");
        ChatCompletionAgent terminatorAgent = kernel.CreateAgent(terminatorAgentCreator);
        return terminatorAgent;
    }
}