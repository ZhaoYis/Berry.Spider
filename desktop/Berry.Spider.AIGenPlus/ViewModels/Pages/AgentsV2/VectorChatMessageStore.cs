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
internal sealed class VectorChatMessageStore : ChatMessageStore
{
    private readonly VectorStore _vectorStore;
    private readonly JsonSerializerOptions _jsonOptions;

    public VectorChatMessageStore(
        VectorStore vectorStore,
        JsonElement serializedStoreState,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        this._vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        if (serializedStoreState.ValueKind is JsonValueKind.String)
        {
            this.ThreadDbKey = serializedStoreState.Deserialize<string>();
        }

        this._jsonOptions = jsonSerializerOptions ?? JsonSerializerOptions.Default;
    }

    public string? ThreadDbKey { get; private set; }

    public override async Task AddMessagesAsync(
        IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        this.ThreadDbKey ??= Guid.NewGuid().ToString("N");
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistoryItem()
        {
            Key = $"{this.ThreadDbKey}_{x.MessageId}",
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = this.ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(
        CancellationToken cancellationToken = default)
    {
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = collection
            .GetAsync(
                x => x.ThreadId == this.ThreadDbKey, 10,
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

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.SerializeToElement(this.ThreadDbKey, jsonSerializerOptions ?? _jsonOptions);

    private sealed class ChatHistoryItem
    {
        [VectorStoreKey] public string? Key { get; set; }
        [VectorStoreData] public string? ThreadId { get; set; }
        [VectorStoreData] public DateTimeOffset? Timestamp { get; set; }
        [VectorStoreData] public string? SerializedMessage { get; set; }
        [VectorStoreData] public string? MessageText { get; set; }
    }
}