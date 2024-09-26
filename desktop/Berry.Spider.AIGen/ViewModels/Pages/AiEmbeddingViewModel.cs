using System.Threading.Tasks;
using AgileConfig.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AiEmbeddingViewModel(ConfigClient configClient, Kernel kernel) : ViewModelBase, ITransientDependency
{
    [ObservableProperty] private string _index = null!;
    [ObservableProperty] private string _input = null!;
    [ObservableProperty] private string _responseText = null!;

    [RelayCommand]
    private async Task RAGReplyAsync()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EmbeddingAsync()
    {
        await Task.CompletedTask;
    }
}