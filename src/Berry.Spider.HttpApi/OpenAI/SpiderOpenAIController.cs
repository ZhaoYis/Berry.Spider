using Berry.Spider.OpenAI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider.OpenAI;

/// <summary>
/// OpenAI爬虫服务
/// </summary>
[Route("api/services/spider/openai")]
public class SpiderOpenAIController : SpiderControllerBase
{
    private readonly ISpiderOpenAIAppService _spiderOpenAiAppService;

    public SpiderOpenAIController(ISpiderOpenAIAppService spiderOpenAiAppService)
    {
        _spiderOpenAiAppService = spiderOpenAiAppService;
    }

    /// <summary>
    /// 文档生成
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("text-generation")]
    public async Task<TextGenerationDto> TextGenerationAsync([FromBody] TextGenerationInput input)
    {
        return await _spiderOpenAiAppService.TextGenerationAsync(input);
    }
}