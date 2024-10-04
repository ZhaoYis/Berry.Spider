using System;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Berry.Spider.AIGen.ViewModels;

public class ViewModelRecipientBase : ObservableRecipient
{
    public event EventHandler<NotificationMessageEventArgs>? ShowNotificationMessageEvent;

    protected virtual void ShowNotificationMessage(NotificationMessageEventArgs e)
    {
        ShowNotificationMessageEvent?.Invoke(this, e);
    }
}