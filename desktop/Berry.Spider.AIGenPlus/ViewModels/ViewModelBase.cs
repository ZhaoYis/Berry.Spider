using System;
using Berry.Spider.AIGenPlus.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Berry.Spider.AIGenPlus.ViewModels;

public class ViewModelBase : ObservableObject
{
    public event EventHandler<NotificationMessageEventArgs>? ShowNotificationMessageEvent;

    protected virtual void ShowNotificationMessage(string message)
    {
        this.ShowNotificationMessage("温馨提示", message);
    }

    protected virtual void ShowNotificationMessage(string messageTitle, string message)
    {
        this.ShowNotificationMessage(new NotificationMessageEventArgs(messageTitle, message));
    }

    protected virtual void ShowNotificationMessage(NotificationMessageEventArgs e)
    {
        ShowNotificationMessageEvent?.Invoke(this, e);
    }
}