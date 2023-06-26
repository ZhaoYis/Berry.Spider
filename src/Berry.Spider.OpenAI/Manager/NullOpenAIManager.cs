namespace Berry.Spider.OpenAI;

public class NullOpenAIManager : IOpenAIManager
{
    public Task<string?> CreateCompletionAsync(string prompt, int? maxTokens)
    {
        return Task.FromResult(default(string));
    }
}