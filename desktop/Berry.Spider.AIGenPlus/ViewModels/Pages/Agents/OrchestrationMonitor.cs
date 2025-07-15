using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.Agents;

public sealed class OrchestrationMonitor
{
    /// <summary>
    /// 聊天历史记录
    /// </summary>
    private ChatHistory ChatHistory = [];

    /// <summary>
    /// 流式聊天历史记录
    /// </summary>
    public List<StreamingChatMessageContent> StreamedResponses = [];

    /// <summary>
    /// 响应内容回调函数
    /// </summary>
    /// <returns></returns>
    public ValueTask ResponseCallback(ChatMessageContent response)
    {
        this.ChatHistory.Add(response);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 流式响应内容回调函数
    /// </summary>
    /// <returns></returns>
    public ValueTask StreamingResultCallback(StreamingChatMessageContent streamedResponse, bool isFinal)
    {
        this.StreamedResponses.Add(streamedResponse);

        if (isFinal)
        {
            this.StreamedResponses.Clear();
        }

        return ValueTask.CompletedTask;
    }
}