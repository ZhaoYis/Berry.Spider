using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Agent实例
    /// </summary>
    protected ChatClientAgent Agent { get; private set; }

    /// <summary>
    /// Agent名称
    /// </summary>
    public abstract string AgentName { get; }

    /// <summary>
    /// Agent类型
    /// </summary>
    public abstract AgentType AgentType { get; }

    /// <summary>
    /// Agent指令(System Prompt)
    /// </summary>
    protected abstract string Instructions { get; }

    /// <summary>
    /// 最大Token数
    /// </summary>
    protected virtual int MaxTokens => 4000;

    /// <summary>
    /// 温度参数
    /// </summary>
    protected virtual float Temperature => 0.7f;

    /// <summary>
    /// Agent的工具函数列表
    /// </summary>
    protected virtual IEnumerable<AITool>? Tools => null;

    /// <summary>
    /// 响应格式(用于结构化输出)
    /// </summary>
    protected virtual ChatResponseFormat? ResponseFormat => null;

    /// <summary>
    /// 执行Agent任务（非流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    public virtual async Task<string> ExecuteAsync(string input, string taskId)
    {
        try
        {
            (ChatClientAgent chatClientAgent, AgentThread agentThread) = this.GetAgentAndNewThread(taskId);
            var result = await chatClientAgent.RunAsync(input, agentThread);
            return result.Text;
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
    public virtual async IAsyncEnumerable<string> ExecuteStreamAsync(string input, string taskId)
    {
        (ChatClientAgent chatClientAgent, AgentThread agentThread) = this.GetAgentAndNewThread(taskId);
        await foreach (var output in chatClientAgent.RunStreamingAsync(input, agentThread))
        {
            if (!string.IsNullOrEmpty(output.Text))
            {
                yield return output.Text;
            }
        }
    }

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns></returns>
    private (ChatClientAgent, AgentThread) GetAgentAndNewThread(string taskId)
    {
        var options = new ChatClientAgentOptions(instructions: Instructions)
        {
            Name = AgentName,
            ChatOptions = new ChatOptions
            {
                MaxOutputTokens = MaxTokens,
                Temperature = Temperature,
                ResponseFormat = ResponseFormat,
                Tools = this.Tools?.ToList()
            }
        };
        this.Agent = this.ChatClient.CreateAIAgent(options);
        AgentThread agentThread = this.Agent.GetNewThread(taskId);
        return (this.Agent, agentThread);
    }
}