using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages;

public partial class EmbeddingViewModel(
    [FromKeyedServices(nameof(OllamaEmbeddingGenerator))]
    OllamaEmbeddingGenerator embeddingGenerator) : ViewModelBase, ITransientDependency
{
    /**
     * docker启动qdrant服务：
     * docker run -p 6333:6333 -p 6334:6334 -v $(pwd)/qdrant_storage:/qdrant/storage qdrant/qdrant
     */
    
    /// <summary>
    /// 待嵌入的文本
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ExecEmbeddingCommand))]
    private string _docText = null!;

    [NotifyCanExecuteChangedFor(nameof(ExecSearchCommand))] [ObservableProperty]
    private string _questionText = null!;

    [ObservableProperty] private string _answerText = null!;

    /// <summary>
    /// 执行文本嵌入
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteEmbedding))]
    private async Task ExecEmbeddingAsync()
    {
        var vectorStore = new QdrantVectorStore(new QdrantClient(host: "localhost", port: 6334));
        var ragVectorRecordCollection = vectorStore.GetCollection<Guid, TextSnippet>("TextSnippet");
        await ragVectorRecordCollection.CreateCollectionIfNotExistsAsync();

        var result = await embeddingGenerator.GenerateEmbeddingVectorAsync(this.DocText);
        TextSnippet textSnippet = new TextSnippet
        {
            Key = Guid.CreateVersion7(),
            Text = this.DocText,
            TextEmbedding = result
        };
        Guid key = await ragVectorRecordCollection.UpsertAsync(textSnippet);
    }

    /// <summary>
    /// 执行相似度搜索
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSearch))]
    private async Task ExecSearchAsync()
    {
        var searchOptions = new VectorSearchOptions
        {
            Top = 3,
            VectorPropertyName = nameof(TextSnippet.TextEmbedding)
        };
        var vectorStore = new QdrantVectorStore(new QdrantClient(host: "localhost", port: 6334));
        var ragVectorRecordCollection = vectorStore.GetCollection<Guid, TextSnippet>("TextSnippet");
        var queryEmbedding = await embeddingGenerator.GenerateEmbeddingVectorAsync(this.QuestionText);
        var searchResults = await ragVectorRecordCollection.VectorizedSearchAsync(queryEmbedding, searchOptions);

        var responseResults = new List<TextSearchResult>();
        await foreach (var result in searchResults.Results)
        {
            responseResults.Add(new TextSearchResult()
            {
                Value = result.Record.Text ?? string.Empty,
                Score = result.Score
            });
        }

        StringBuilder response = new StringBuilder();
        foreach (TextSearchResult result in responseResults)
        {
            this.AnswerText = response.AppendLine(result.Value).ToString();
        }
    }

    private bool CanExecuteEmbedding()
    {
        return !string.IsNullOrWhiteSpace(this.DocText);
    }

    private bool CanExecuteSearch()
    {
        return !string.IsNullOrWhiteSpace(this.QuestionText);
    }

    public class TextSnippet
    {
        /// <summary>
        /// 文档主键
        /// </summary>
        [VectorStoreRecordKey]
        public required Guid Key { get; set; }

        /// <summary>
        /// 文档原始内容
        /// </summary>
        [VectorStoreRecordData]
        public required string Text { get; set; }

        /// <summary>
        /// 文档向量（1024维）
        /// </summary>
        [VectorStoreRecordVector(Dimensions: 1024)]
        public required ReadOnlyMemory<float> TextEmbedding { get; set; }
    }

    public class TextSearchResult
    {
        public string Value { get; set; }
        public double? Score { get; set; }
    }
}