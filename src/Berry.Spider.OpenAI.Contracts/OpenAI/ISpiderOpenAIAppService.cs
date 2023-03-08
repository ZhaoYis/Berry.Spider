using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.OpenAI.Contracts;

public interface ISpiderOpenAIAppService : IApplicationService, ITransientDependency
{
    /// <summary>
    /// 文档生成
    /// </summary>
    /// <returns></returns>
    Task<TextGenerationDto> TextGenerationAsync(TextGenerationInput input);
}