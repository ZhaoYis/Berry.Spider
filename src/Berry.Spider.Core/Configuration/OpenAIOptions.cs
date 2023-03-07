namespace Berry.Spider.Contracts;

public class OpenAIOptions
{
    /// <summary>
    /// 是否启用OpenAI
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// ApiKey
    /// </summary>
    public string ApiKey { get; set; }
}