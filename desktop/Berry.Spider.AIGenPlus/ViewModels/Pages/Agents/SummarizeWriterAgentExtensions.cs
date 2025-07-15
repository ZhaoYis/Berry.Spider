using Berry.Spider.AIGenPlus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

public static class SummarizeWriterAgentExtensions
{
    /// <summary>
    /// SummarizeWriterAgent：摘要写手。根据提供的关键词或者句子生成一段语义丰富的摘要信息，以供下一个agent根据摘要信息编写主体内容
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static ChatCompletionAgent CreateSummarizeWriterAgent(this Kernel kernel)
    {
        ChatAgentCreator summarizeWriterAgentCreator = new("SummarizeWriterAgent",
                                                           """

                                                           """,
                                                           "");
        ChatCompletionAgent summarizeWriterAgent = kernel.CreateAgent(summarizeWriterAgentCreator);
        return summarizeWriterAgent;
    }
}