using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    private ChatClientAgent Agent { get; set; }

    /// <summary>
    /// Agent名称
    /// </summary>
    public abstract string AgentName { get; }

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
    /// 恢复之前的对话
    /// </summary>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <param name="chatClientAgent">聊天客户端Agent</param>
    /// <returns></returns>
    protected virtual async Task<AgentThread> ResumePreviousConversationAsync(string taskId, ChatClientAgent chatClientAgent)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{taskId}_agent_thread.json");
        var serializedThread = await File.ReadAllTextAsync(filePath);
        // 反序列化Agent线程状态
        if (string.IsNullOrEmpty(serializedThread))
        {
            return null;
        }

        return chatClientAgent.DeserializeThread(JsonSerializer.Deserialize<JsonElement>(serializedThread));
    }

    /// <summary>
    /// 保存Agent线程状态
    /// </summary>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <param name="agentThread">Agent线程</param>
    /// <returns></returns>
    protected virtual Task SaveThreadStateAsync(string taskId, AgentThread agentThread)
    {
        // 序列化并保存当前对话状态到持久存储（例如文件、数据库等）
        var serializedThread = agentThread.Serialize(JsonSerializerOptions.Web).GetRawText();
        var filePath = Path.Combine(Path.GetTempPath(), $"{taskId}_agent_thread.json");
        return File.WriteAllTextAsync(filePath, serializedThread);
    }

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
            // 恢复之前的对话
            AgentThread reloadedThread = await this.ResumePreviousConversationAsync(taskId, chatClientAgent) ?? agentThread;
            // 执行Agent任务
            var result = await chatClientAgent.RunAsync(input, reloadedThread);
            // 保存Agent线程状态
            await this.SaveThreadStateAsync(taskId, reloadedThread);
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
        // 恢复之前的对话
        AgentThread reloadedThread = await this.ResumePreviousConversationAsync(taskId, chatClientAgent) ?? agentThread;
        await foreach (var output in chatClientAgent.RunStreamingAsync(input, agentThread))
        {
            if (!string.IsNullOrEmpty(output.Text))
            {
                yield return output.Text;
            }
        }

        // 保存Agent线程状态
        await this.SaveThreadStateAsync(taskId, reloadedThread);
    }

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns></returns>
    public virtual AIAgent GetAgent()
    {
        return this.Agent ?? throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
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