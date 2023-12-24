using Berry.Spider.OpenAI.Contracts;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Berry.Spider.OpenAI;

/// <summary>
/// OpenAI爬虫服务
/// </summary>
[Area(GlobalConstants.ModelName)]
[Route("api/services/spider/openai")]
[RemoteService(Name = GlobalConstants.RemoteServiceName)]
public class SpiderOpenAIController : SpiderControllerBase, ISpiderOpenAIAppService
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