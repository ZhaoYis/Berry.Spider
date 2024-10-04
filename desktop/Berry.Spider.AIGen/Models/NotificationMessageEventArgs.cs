using System;
using System.Diagnostics;
using Avalonia.Controls.Notifications;

namespace Berry.Spider.AIGen.Models;

public class NotificationMessageEventArgs(string messageTitle, string message) : EventArgs
{
    public string MessageTitle { get; set; } = messageTitle;
    public string Message { get; set; } = message;
    public NotificationType MessageType { get; set; } = NotificationType.Information;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(3);
    public Action? OnClick { get; set; } = () => { Debug.WriteLine("on clicked..."); };
    public Action? OnClose { get; set; } = () => { Debug.WriteLine("on closed..."); };
}