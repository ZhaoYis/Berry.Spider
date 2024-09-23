using System.Text;
using System.Threading.Tasks;
using AgileConfig.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ITransientDependency
{
    private readonly ConfigClient _configClient;
    private readonly Kernel _kernel;

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(ConfigClient client, Kernel kernel) : this()
    {
        _configClient = client;
        _kernel = kernel;
    }

    /// <summary>
    /// 问AI的问题文本
    /// </summary>
    [ObservableProperty] private string _askAiRequestText;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText;

    /// <summary>
    /// 问AI
    /// </summary>
    [RelayCommand]
    private async Task AskAiAsync()
    {
        ITextGenerationService textGenerationService = _kernel.GetRequiredService<ITextGenerationService>();
        StringBuilder article = new StringBuilder();
        await foreach (var text in textGenerationService.GetStreamingTextContentsAsync(this.AskAiRequestText))
        {
            article.Append(text.Text);
            this.AskAiResponseText = article.ToString();
        }

        await Task.CompletedTask;
    }
}