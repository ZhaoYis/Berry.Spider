using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;

namespace Berry.Spider.Core;

/// <summary>
/// SignalR扩展方法
/// </summary>
public static class HubConnectionExtensions
{
    /// <summary>
    /// 发消息
    /// </summary>
    /// <returns></returns>
    public static async Task SendToAsync<T>(this HubConnection connection, [Required] T data)
    {
        await connection.SendAsync(typeof(T).GetMethodName(), data);
    }
}