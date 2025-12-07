using System.Text.Json.Serialization;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages.AgentsV2;

public class MainWriterOutput
{
    // 文章标题
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

    // 文章摘要
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    //文章内容
    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}