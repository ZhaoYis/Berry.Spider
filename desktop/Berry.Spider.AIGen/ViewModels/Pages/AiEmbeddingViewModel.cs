using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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

public partial class AiEmbeddingViewModel(Kernel kernel, ISemanticTextMemory textMemory) : ViewModelRecipientBase, ITransientDependency
{
    /// <summary>
    /// 存储文本向量集合名称
    /// </summary>
    [ObservableProperty] private string _collectionName = "demo";

    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AskAiCommand))] [NotifyCanExecuteChangedFor(nameof(EmbeddingCommand))]
    private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    /// <summary>
    /// 问AI
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task AskAiAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        this.IsActive = true;
        this.Messenger.Send(new NotificationTaskMessage(isRunning: true));

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

        this.Messenger.Send(new NotificationTaskMessage(isRunning: false));
    }

    /// <summary>
    /// 执行文本嵌入
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task EmbeddingAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));

        var lines = TextChunker.SplitPlainTextLines(this.AskAiRequestText, 100);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 1000);
        foreach (var para in paragraphs)
        {
            await textMemory.SaveInformationAsync(this.CollectionName, id: Guid.CreateVersion7().ToString(), text: para, cancellationToken: default);
        }

        this.ShowNotificationMessage(new NotificationMessageEventArgs("温馨提示", "文本嵌入操作成功"));
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

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(this.AskAiRequestText);
    }
}