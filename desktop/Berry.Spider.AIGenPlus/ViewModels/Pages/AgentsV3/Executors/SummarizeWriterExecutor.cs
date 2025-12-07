using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV3.Executors;

public sealed class SummarizeWriterExecutor(string id, ISummarizeWriterAgent summarizeWriterAgent)
    : ReflectingExecutor<SummarizeWriterExecutor>(id), IMessageHandler<string, SummarizeOutput>
{
    // protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    // {
    //     return routeBuilder.AddHandler<string, SummarizeOutput>(this.HandleAsync);
    // }

    public async ValueTask<SummarizeOutput> HandleAsync(string message, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = $"""
                         请根据以下任务要求生成文章摘要：
                         {message}
                         """;
        string instructions = summarizeWriterAgent.GetCustomOrDefaultInstructions(prompt);
        string result = await summarizeWriterAgent.ExecuteAsync(instructions, this.Id);
        SummarizeOutput? summarizeOutput = JsonSerializer.Deserialize<SummarizeOutput>(result);
        if (summarizeOutput is null)
        {
            throw new JsonException($"无法将 JSON 字符串反序列化为 {nameof(SummarizeOutput)} 类型。");
        }

        //发布事件
        await context.AddEventAsync(new SummarizeWriterFinishedEvent(summarizeOutput), cancellationToken);
        return summarizeOutput;
    }
}