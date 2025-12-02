using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

/// <summary>
/// 提供当前日期时间的AI上下文
/// </summary>
internal sealed class CurrentDateTimeAIContextProvider(IChatClient chatClient) : AIContextProvider
{
    public override ValueTask<AIContext> InvokingAsync(InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = $"当前日期时间为：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"
        });
    }
}