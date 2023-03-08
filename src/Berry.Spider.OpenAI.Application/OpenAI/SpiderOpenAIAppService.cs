using Berry.Spider.OpenAI.Contracts;
using Volo.Abp.Application.Services;

namespace Berry.Spider.OpenAI.Application;

public class SpiderOpenAIAppService : ApplicationService, ISpiderOpenAIAppService
{
    private readonly IOpenAIManager _openAiManager;

    public SpiderOpenAIAppService(IOpenAIManager openAiManager)
    {
        _openAiManager = openAiManager;
    }

    /// <summary>
    /// 文档生成
    /// </summary>
    /// <returns></returns>
    public async Task<TextGenerationDto> TextGenerationAsync(TextGenerationInput input)
    {
        string? text = await _openAiManager.CreateCompletionAsync(input.Keyword, input.MaxLength);
        if (!string.IsNullOrEmpty(text))
        {
            return new TextGenerationDto { Content = text };
        }

        return new TextGenerationDto { Content = "非常抱歉，宇宙最强AI都没办法理解您的问题！请重新描述一下你的问题。" };
    }
}