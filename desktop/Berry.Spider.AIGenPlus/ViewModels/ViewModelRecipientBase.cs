using System;
using Berry.Spider.AIGenPlus.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Berry.Spider.AIGenPlus.ViewModels;

public class ViewModelRecipientBase : ObservableRecipient
{
    public event EventHandler<NotificationMessageEventArgs>? ShowNotificationMessageEvent;

    protected virtual void ShowNotificationMessage(NotificationMessageEventArgs e)
    {
        ShowNotificationMessageEvent?.Invoke(this, e);
    }
}