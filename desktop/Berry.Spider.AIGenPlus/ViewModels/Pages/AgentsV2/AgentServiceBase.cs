using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Volo.Abp;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

/// <summary>
/// 基础Agent服务类
/// </summary>
public abstract class AgentServiceBase(IChatClient chatClient) : IAgentService
{
    /// <summary>
    /// 聊天客户端
    /// </summary>
    private IChatClient ChatClient { get; } = chatClient;

    /// <summary>
    /// Agent客户端实例
    /// </summary>
    private ChatClientAgent? ChatClientAgent { get; set; }

    /// <summary>
    /// Agent名称
    /// </summary>
    public abstract string AgentName { get; }

    /// <summary>
    /// Agent执行顺序
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// Agent指令(System Prompt)
    /// </summary>
    protected abstract string Instructions { get; }

    /// <summary>
    /// 最大Token数。作用是限制模型生成的Token数量，防止生成过长的文本。默认值为4000。
    /// </summary>
    protected virtual int MaxTokens => 4000;

    /// <summary>
    /// 温度参数。作用是控制模型生成Token的随机性。值越高，生成的Token越随机；值越低，生成的Token越确定。默认值为0.7f。
    /// </summary>
    protected virtual float Temperature => 0.7f;

    /// <summary>
    /// 频率惩罚参数。作用是惩罚模型生成重复Token的概率。默认值为0.5f。
    /// </summary>
    protected virtual float? FrequencyPenalty => 0.5f;

    /// <summary>
    /// Top-K参数。作用是限制模型生成的Token数量，只保留概率最高的K个Token。默认值为50。
    /// </summary>
    protected virtual int? TopK => 50;

    /// <summary>
    /// Top-P参数。作用是限制模型生成的Token数量，只保留概率最高的P%个Token。默认值为0.7f。
    /// </summary>
    protected virtual float? TopP => 0.7f;

    /// <summary>
    /// Agent的工具函数列表。作用是为模型提供额外的功能，例如调用外部API、执行计算等。
    /// </summary>
    protected virtual IEnumerable<AITool>? Tools => null;

    /// <summary>
    /// 响应格式(用于结构化输出)
    /// </summary>
    protected virtual ChatResponseFormat? ResponseFormat => null;

    /// <summary>
    /// 锁
    /// </summary>
    private static readonly Lock Lock = new();

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns></returns>
    [Experimental("MEAI001")]
    public virtual AIAgent GetAgent()
    {
        Lock.Enter();
        try
        {
            if (this.ChatClientAgent is not null)
            {
                return this.ChatClientAgent;
            }

            return this.ChatClientAgent = this.CreateNewAIAgent() ??
                                          throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
        }
        finally
        {
            Lock.Exit();
        }
    }

    /// <summary>
    /// 获取自定义指令或默认指令
    /// </summary>
    /// <param name="instructions">自定义指令</param>
    /// <returns>自定义指令或默认指令</returns>
    public virtual string GetCustomOrDefaultInstructions(string? instructions = null)
    {
        return string.IsNullOrEmpty(instructions) ? Instructions : $"{Instructions}{Environment.NewLine}{instructions}";
    }

    /// <summary>
    /// 创建AI上下文提供程序（默认实现为当前日期时间上下文提供程序）
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<AIContextProvider> CreateAIContextProviders()
    {
        return [new CurrentDateTimeAIContextProvider(this.ChatClient)];
    }

    /// <summary>
    /// 执行Agent任务（非流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    [Experimental("MEAI001")]
    public virtual async Task<string> ExecuteAsync(string input, string taskId)
    {
        try
        {
            ChatClientAgent chatClientAgent = this.GetAgent() as ChatClientAgent ??
                                              throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
            var result = await chatClientAgent.RunAsync(input);
            return result.Text;
        }
        catch (Exception e)
        {
            throw new BusinessException($"Agent执行任务失败，任务ID：{taskId}，错误信息：{e.Message}");
        }
    }

    /// <summary>
    /// 执行Agent任务（非流式）泛型
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    [Experimental("MEAI001")]
    public async Task<T> ExecuteAsync<T>(string input, string taskId)
    {
        try
        {
            ChatClientAgent chatClientAgent = this.GetAgent() as ChatClientAgent ??
                                              throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
            var result = await chatClientAgent.RunAsync<T>(input);
            return result.Result;
        }
        catch (Exception e)
        {
            throw new BusinessException($"Agent执行任务失败，任务ID：{taskId}，错误信息：{e.Message}");
        }
    }

    /// <summary>
    /// 执行Agent任务（流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    [Experimental("MEAI001")]
    public virtual async IAsyncEnumerable<string> ExecuteStreamAsync(string input, string taskId)
    {
        ChatClientAgent chatClientAgent = this.GetAgent() as ChatClientAgent ??
                                          throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
        await foreach (var output in chatClientAgent.RunStreamingAsync(input))
        {
            if (!string.IsNullOrEmpty(output.Text))
            {
                yield return output.Text;
            }
        }
    }

    /// <summary>
    /// 解析JSON响应
    /// </summary>
    protected T? ParseJsonResponse<T>(string jsonContent) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            // 如果失败,尝试提取JSON代码块
            var startIndex = jsonContent.IndexOf('{');
            var endIndex = jsonContent.LastIndexOf('}');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                var jsonStr = jsonContent.Substring(startIndex, endIndex - startIndex + 1);
                return JsonSerializer.Deserialize<T>(jsonStr, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }
    }

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns></returns>
    [Experimental("MEAI001")]
    private ChatClientAgent CreateNewAIAgent()
    {
        var options = new ChatClientAgentOptions
        {
            Name = this.AgentName,
            ChatOptions = new ChatOptions
            {
                Instructions = this.Instructions,
                MaxOutputTokens = this.MaxTokens,
                Temperature = this.Temperature,
                TopP = this.TopP,
                TopK = this.TopK,
                FrequencyPenalty = this.FrequencyPenalty,
                ResponseFormat = this.ResponseFormat,
                Tools = this.Tools?.ToList()
            },
            WarnOnChatHistoryProviderConflict = false,
            ThrowOnChatHistoryProviderConflict = false,
            ClearOnChatHistoryProviderConflict = true,
            AIContextProviders = this.CreateAIContextProviders(),
            ChatHistoryProvider = new InMemoryChatHistoryProvider(new InMemoryChatHistoryProviderOptions
            {
                //ChatReduction 仅适用于消息存储在本地的场景（如InMemoryChatHistoryProvider）。对于消息存储在服务端的场景（如 Azure Foundry Agents），聊天历史由服务自身管理，客户端无法干预裁剪。
                ChatReducer = new MessageCountingChatReducer(10),
                ReducerTriggerEvent = InMemoryChatHistoryProviderOptions.ChatReducerTriggerEvent.BeforeMessagesRetrieval
            })
        };
        return this.ChatClient.AsAIAgent(options);
    }
}