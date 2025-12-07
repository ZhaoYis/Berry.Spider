using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public partial class ArticleWriterAgentV3ViewModel(
    ISummarizeWriterAgent summarizeWriterAgent,
    IMainWriterAgent mainWriterAgent,
    IReviewerAgent reviewerAgent) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 用户输入
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GeneratingCommand))]
    private string? _userInput;

    /// <summary>
    /// AI生成的内容
    /// </summary>
    [ObservableProperty] private string? _aiResponseText;

    /// <summary>
    /// 开始生成
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task GeneratingAsync()
    {
        Check.NotNullOrWhiteSpace(this.UserInput, nameof(UserInput));
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");
    }

    /// <summary>
    /// 是否可以执行
    /// </summary>
    /// <returns></returns>
    private bool CanExecute() => !string.IsNullOrWhiteSpace(this.UserInput);
}