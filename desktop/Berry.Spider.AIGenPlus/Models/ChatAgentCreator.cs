namespace Berry.Spider.AIGenPlus.Models;

/// <summary>
/// agent初始器
/// </summary>
/// <param name="AgentName">代理名称</param>
/// <param name="Instructions">指令</param>
/// <param name="Description">描述信息</param>
public record ChatAgentCreator(string AgentName, string Instructions, string? Description = null);