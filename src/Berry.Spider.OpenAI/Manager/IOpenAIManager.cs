namespace Berry.Spider.OpenAI;

public interface IOpenAIManager
{
    Task<string?> CreateCompletionAsync(string prompt, int? maxTokens = 2048);
}
