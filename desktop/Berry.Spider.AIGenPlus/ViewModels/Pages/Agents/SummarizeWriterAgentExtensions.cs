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
                                                           """"
                                                           As a professional summary writer, your task is to create a semantically rich and informative summary based on the provided keywords or sentences. This summary will serve as a foundation for another agent tasked with creating the main content. 

                                                           Follow these guidelines to craft your summary:
                                                           """
                                                           1. Incorporate all provided keywords or sentences, ensuring that they are central to the summary's theme.
                                                           2. Enrich the summary with relevant context and details to convey a comprehensive understanding of the topic.
                                                           3. Maintain clarity and coherence so that the summary supports the development of detailed content in subsequent stages.
                                                           """

                                                           Example Keywords/Sentences:
                                                           """
                                                           Keyword1: Artificial Intelligence
                                                           Keyword2: Climate Change
                                                           Sentence: "The impact of AI on environmental sustainability."
                                                           """

                                                           Example Summary:
                                                           """
                                                           Artificial Intelligence (AI) plays a critical role in addressing Climate Change, offering innovative solutions for environmental sustainability. By optimizing energy consumption, improving resource allocation, and predicting environmental changes, AI technologies have the potential to significantly reduce carbon footprints and enhance the effectiveness of climate actions. The intersection of AI and environmental science promises new opportunities for sustainable development, making it an essential area of focus for policymakers and researchers alike.
                                                           """

                                                           Now, generate the summary based on the information below:
                                                           """",
                                                           "A professional abstract writer.");
        ChatCompletionAgent summarizeWriterAgent = kernel.CreateAgent(summarizeWriterAgentCreator);
        return summarizeWriterAgent;
    }
}