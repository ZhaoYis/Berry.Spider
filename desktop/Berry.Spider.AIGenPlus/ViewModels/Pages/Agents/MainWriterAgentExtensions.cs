using Berry.Spider.AIGenPlus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

public static class MainWriterAgentExtensions
{
    /// <summary>
    /// MainWriterAgent：主要内容写手。根据摘要信息，生成主要内容
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static ChatCompletionAgent CreateMainWriterAgent(this Kernel kernel)
    {
        ChatAgentCreator mainWriterAgentCreator = new("MainWriterAgent",
                                                      """

                                                      """,
                                                      "");
        ChatCompletionAgent mainWriterAgent = kernel.CreateAgent(mainWriterAgentCreator);
        return mainWriterAgent;
    }
}