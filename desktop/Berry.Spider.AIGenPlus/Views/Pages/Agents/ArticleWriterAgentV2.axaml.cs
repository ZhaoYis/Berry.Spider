using Avalonia.Controls;
using Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.AIGenPlus.Views.Pages.Agents;

public partial class ArticleWriterAgentV2 : UserControlBase
{
    private bool _userAtBottom = true;

    public ArticleWriterAgentV2()
    {
        ArticleWriterAgentV2ViewModel vm = App.Current.GetRequiredService<ArticleWriterAgentV2ViewModel>();
        vm.ShowNotificationMessageEvent += this.ShowNotificationMessage;
        this.DataContext = vm;
        InitializeComponent();

        if (this.FindControl<TextBox>("AiResponseTextBox") is { } aiResponseTextBox)
        {
            aiResponseTextBox.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                {
                    if (_userAtBottom)
                    {
                        var textLength = aiResponseTextBox.Text?.Length ?? 0;
                        aiResponseTextBox.CaretIndex = textLength;
                    }
                }
                else if (e.Property == TextBox.CaretIndexProperty)
                {
                    var textLength = aiResponseTextBox.Text?.Length ?? 0;
                    _userAtBottom = aiResponseTextBox.CaretIndex >= textLength;
                }
            };
        }
    }
}