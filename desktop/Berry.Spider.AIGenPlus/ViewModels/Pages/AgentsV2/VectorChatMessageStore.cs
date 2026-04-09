using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

/// <summary>
/// 向量聊天消息存储
/// </summary>
internal sealed class VectorChatMessageStore(VectorStore vectorStore) : ChatHistoryProvider
{
    private readonly VectorStore _vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));

    protected override async ValueTask StoreChatHistoryAsync(InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(context.ResponseMessages?.Select(x => new ChatHistoryItem()
        {
            Key = $"{context.Agent.Id}_{x.MessageId}",
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = context.Agent.Id,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        })!, cancellationToken);
    }

    protected override ValueTask<IEnumerable<ChatMessage>> InvokingCoreAsync(InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        return base.InvokingCoreAsync(context, cancellationToken);
    }

    protected override ValueTask InvokedCoreAsync(InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        return base.InvokedCoreAsync(context, cancellationToken);
    }

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = collection
            .GetAsync(
                x => x.ThreadId == context.Agent.Id, 10,
                new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken);

        List<ChatMessage> messages = [];
        await foreach (var record in records)
        {
            messages.Add(JsonSerializer.Deserialize<ChatMessage>(record.SerializedMessage!)!);
        }

        messages.Reverse();
        return messages;
    }

    private sealed class ChatHistoryItem
    {
        [VectorStoreKey] public string? Key { get; set; }
        [VectorStoreData] public string? ThreadId { get; set; }
        [VectorStoreData] public DateTimeOffset? Timestamp { get; set; }
        [VectorStoreData] public string? SerializedMessage { get; set; }
        [VectorStoreData] public string? MessageText { get; set; }
    }
}