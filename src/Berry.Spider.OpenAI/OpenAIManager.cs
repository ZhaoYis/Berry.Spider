using OpenAI.GPT3.Interfaces;

namespace Berry.Spider.OpenAI;

public class OpenAIManager : IOpenAIManager
{
    private readonly IOpenAIService _openAiService;

    public OpenAIManager(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }
}