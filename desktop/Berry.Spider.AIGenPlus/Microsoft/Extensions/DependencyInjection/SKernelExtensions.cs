using Berry.Spider.AIGenPlus.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Volo.Abp;

namespace Microsoft.Extensions.DependencyInjection;

public static class SKernelExtensions
{
    /// <summary>
    /// 实例化一个agent
    /// </summary>
    /// <returns></returns>
    public static ChatCompletionAgent CreateAgent(this Kernel kernel, ChatAgentCreator creator)
    {
        Check.NotNull(kernel, nameof(kernel));
        Check.NotNull(creator, nameof(creator));

        return new ChatCompletionAgent
        {
            Name = creator.AgentName,
            Instructions = "{{#ins}}",
            Description = creator.Description,
            Kernel = kernel,
            Arguments = new KernelArguments(new OllamaPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
            {
                ["ins"] = creator.Instructions
            }
        };
    }
}