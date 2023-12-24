using Volo.Abp.Application.Services;

namespace Berry.Spider.OpenAI.Contracts;

public interface ISpiderOpenAIAppService : IApplicationService
{
    /// <summary>
    /// 文档生成
    /// </summary>
    /// <returns></returns>
    Task<TextGenerationDto> TextGenerationAsync(TextGenerationInput input);
}