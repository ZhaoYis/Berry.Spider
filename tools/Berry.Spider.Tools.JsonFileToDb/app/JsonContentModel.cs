using Berry.Spider.Core;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Berry.Spider.Tools.JsonFileToDb;

public class JsonContentModel
{
    [JsonPropertyName("keywords")] public string keywords { get; set; }

    [JsonPropertyName("dropdowns")] public List<string> Dropdowns { get; set; }

    [JsonPropertyName("recommends")] public List<string> Recommends { get; set; }

    [JsonPropertyName("relates")] public List<string> Pelates { get; set; }

    [JsonPropertyName("posts")] public List<PostItem> Posts { get; set; }
}

public class PostItem
{
    [JsonPropertyName("url")] public string Url { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("keywords")] public string Keywords { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    private static readonly Regex ReplaceRegex = new Regex(@"^\d+(、|\.|\．|,|，)*");

    public List<string> GetContentList()
    {
        List<string> contents = new();

        if (string.IsNullOrWhiteSpace(this.Content)) return contents;
        List<string> res = this.Content.Split('\n', '\r').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        foreach (string item in res)
        {
            string trimItem = item.Trim();
            if (ReplaceRegex.IsMatch(trimItem))
            {
                string newItem = ReplaceRegex.Replace(trimItem, "").Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(newItem) && newItem.Length >= 5)
                {
                    contents.Add(newItem);
                }
            }
        }

        contents.RandomSort();

        return contents;
    }
}