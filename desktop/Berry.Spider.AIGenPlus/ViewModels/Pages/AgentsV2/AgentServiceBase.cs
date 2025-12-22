using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
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
    /// 恢复之前的对话
    /// </summary>
    /// <param name="taskId">任务ID(用于记录执行日志)</param>
    /// <param name="chatClientAgent">聊天客户端Agent</param>
    /// <returns></returns>
    protected virtual async Task<AgentThread?> ResumePreviousConversationAsync(string taskId,
        ChatClientAgent chatClientAgent)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "AgentThreads", $"{taskId}.json");
        if (!File.Exists(filePath))
        {
            // 首次执行或尚未保存过线程状态，直接返回 null 使用新线程
            return null;
        }

        try
        {
            var serializedThread = await File.ReadAllTextAsync(filePath);
            // 反序列化Agent线程状态
            if (string.IsNullOrEmpty(serializedThread))
            {
                return null;
            }

            var json = JsonSerializer.Deserialize<JsonElement>(serializedThread);
            return chatClientAgent.DeserializeThread(json);
        }
        catch (Exception e)
        {
            // 状态文件损坏或反序列化失败时，不中断执行，记录诊断信息并回退为新线程
            System.Diagnostics.Debug.WriteLine(
                $"[AgentServiceBase] 恢复线程状态失败，taskId={taskId}，filePath={filePath}，error={e.Message}");
            return null;
        }
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
        var directory = Path.Combine(AppContext.BaseDirectory, "AgentThreads");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, $"{taskId}.json");
        return File.WriteAllTextAsync(filePath, serializedThread);
    }

    /// <summary>
    /// 创建AI上下文提供程序（默认实现为当前日期时间上下文提供程序）
    /// </summary>
    /// <returns></returns>
    protected virtual Func<ChatClientAgentOptions.AIContextProviderFactoryContext, AIContextProvider>
        CreateAIContextProvider()
    {
        return ctx => new CurrentDateTimeAIContextProvider(this.ChatClient);
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
            ChatClientAgent chatClientAgent = this.GetAgent() as ChatClientAgent ??
                                              throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
            AgentThread agentThread = chatClientAgent.GetNewThread();
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
        ChatClientAgent chatClientAgent = this.GetAgent() as ChatClientAgent ??
                                          throw new BusinessException($"Agent实例未初始化，Agent名称：{AgentName}");
        AgentThread agentThread = chatClientAgent.GetNewThread();
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
            ChatMessageStoreFactory = ctx => new VectorChatMessageStore(
                new InMemoryVectorStore(),
                ctx.SerializedState,
                ctx.JsonSerializerOptions),
            AIContextProviderFactory = this.CreateAIContextProvider()
        };
        return this.ChatClient.CreateAIAgent(options);
    }
}