using System.Text;
using System.Threading.Tasks;
using AgileConfig.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AiChatViewModel(ConfigClient configClient, Kernel kernel) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    /// <summary>
    /// 问AI
    /// </summary>
    [RelayCommand]
    private async Task AskAiAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        ITextGenerationService textGenerationService = kernel.GetRequiredService<ITextGenerationService>();
        StringBuilder response = new StringBuilder();
        await foreach (var text in textGenerationService.GetStreamingTextContentsAsync(this.AskAiRequestText))
        {
            this.AskAiResponseText = response.Append(text.Text).ToString();
        }
    }
}