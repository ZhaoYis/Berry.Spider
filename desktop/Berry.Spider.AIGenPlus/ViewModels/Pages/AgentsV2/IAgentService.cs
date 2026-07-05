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
    /// Agent执行顺序
    /// </summary>
    int Order { get; }

    /// <summary>
    /// 获取Agent实例
    /// </summary>
    /// <returns>Agent实例</returns>
    AIAgent GetAgent();

    /// <summary>
    /// 获取自定义指令或默认指令
    /// </summary>
    /// <param name="instructions">自定义指令</param>
    /// <returns>自定义指令或默认指令</returns>
    string GetCustomOrDefaultInstructions(string? instructions = null);

    /// <summary>
    /// 执行Agent任务（非流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    Task<string> ExecuteAsync(string input, string taskId);

    /// <summary>
    /// 执行Agent任务（非流式）泛型
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    Task<T> ExecuteAsync<T>(string input, string taskId);

    /// <summary>
    /// 执行Agent任务（流式）
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <returns>输出内容</returns>
    IAsyncEnumerable<string> ExecuteStreamAsync(string input, string taskId);
}