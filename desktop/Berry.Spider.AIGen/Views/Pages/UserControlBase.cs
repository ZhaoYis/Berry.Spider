using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Berry.Spider.AIGen.Models;

namespace Berry.Spider.AIGen.Views.Pages;

public class UserControlBase : UserControl
{
    private WindowNotificationManager? NotificationManager { get; set; }

    protected void ShowNotificationMessage(object? sender, NotificationMessageEventArgs e)
    {
        NotificationManager?.Show(new Notification(e.MessageTitle, e.Message, e.MessageType, e.Expiration, e.OnClick, e.OnClose));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        NotificationManager = new WindowNotificationManager(topLevel)
        {
            MaxItems = 3,
            Position = NotificationPosition.BottomRight
        };
    }
}