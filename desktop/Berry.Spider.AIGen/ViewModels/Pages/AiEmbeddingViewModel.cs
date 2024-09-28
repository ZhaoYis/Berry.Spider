using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AgileConfig.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Text;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels.Pages;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0010

public partial class AiEmbeddingViewModel(ConfigClient configClient, Kernel kernel, ISemanticTextMemory textMemory) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 存储文本向量集合名称
    /// </summary>
    [ObservableProperty] private string _collectionName = "demo";

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

        var memoryResults = textMemory.SearchAsync(this.CollectionName, this.AskAiRequestText, limit: 3, minRelevanceScore: 0.3);
        var existingKnowledge = await this.BuildPromptInformationAsync(memoryResults);
        var integratedPrompt = """
                               获取到的相关信息：[{0}]。
                               根据获取到的信息回答问题：[{1}]。
                               如果没有获取到相关信息，请直接回答不知道。
                               """;
        string promat = string.Format(integratedPrompt, existingKnowledge, this.AskAiRequestText);
        StringBuilder response = new StringBuilder();
        await foreach (var text in kernel.InvokePromptStreamingAsync<string>(promat))
        {
            this.AskAiResponseText = response.Append(text).ToString();
        }
    }

    /// <summary>
    /// 执行文本嵌入
    /// </summary>
    [RelayCommand]
    private async Task EmbeddingAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        var lines = TextChunker.SplitPlainTextLines(this.AskAiRequestText, 100);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 1000);
        foreach (var para in paragraphs)
        {
            await textMemory.SaveInformationAsync(this.CollectionName, id: Guid.NewGuid().ToString(), text: para, cancellationToken: default);
        }
    }

    /// <summary>
    /// 根据嵌入的结果构建提示词文本信息
    /// </summary>
    /// <returns></returns>
    private async Task<string> BuildPromptInformationAsync(IAsyncEnumerable<MemoryQueryResult> memoryResults)
    {
        StringBuilder information = new StringBuilder();
        await foreach (MemoryQueryResult memoryResult in memoryResults)
        {
            information.Append(memoryResult.Metadata.Text);
        }

        return information.ToString();
    }
}