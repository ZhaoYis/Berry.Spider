using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Agents.AI;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public interface IAgentService
{
    /// <summary>
    /// Agent名称
    /// </summary>
    string AgentName { get; }

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns>Agent实例</returns>
    AIAgent GetAgent();

    /// <summary>
    /// 执行Agent任务（非流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    Task<string> ExecuteAsync(string input, string taskId);

    /// <summary>
    /// 执行Agent任务（流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    IAsyncEnumerable<string> ExecuteStreamAsync(string input, string taskId);
}