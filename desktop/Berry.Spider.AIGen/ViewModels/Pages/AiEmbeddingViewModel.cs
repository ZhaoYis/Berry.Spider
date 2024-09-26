using AgileConfig.Client;
using Microsoft.SemanticKernel;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

public partial class AiEmbeddingViewModel(ConfigClient configClient, Kernel kernel) : ViewModelBase, ITransientDependency
{
}