using Berry.Spider.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using OpenAI.ObjectModels;
using Volo.Abp.Modularity;

namespace Berry.Spider.OpenAI;

public class SpiderOpenAIModule : AbpModule
{
    /**
     * https://platform.openai.com/docs/introduction/overview
     * https://github.com/betalgo/openai/wiki
     */
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        OpenAIOptions? openAiOptions = configuration.GetSection(nameof(OpenAIOptions)).Get<OpenAIOptions>();
        if (openAiOptions is { IsEnabled: true })
        {
            context.Services.AddOpenAIService(opt =>
            {
                opt.ApiKey = openAiOptions.ApiKey;
                //模型参考文档地址：https://platform.openai.com/docs/models/overview
                /**
                 * 以下内容来自ChatGPT的回答：
                 *  TextDavinciV3是OpenAI的一个文本生成模型，它是Davinci模型系列中的一员。TextDavinciV3是一个强大的文本生成模型，可以生成高质量、连贯的文本，包括长文本、故事、剧本等。
                    TextDavinciV3模型的适用场景包括但不限于：
                        创意写作：使用TextDavinciV3模型可以生成有创意、有趣味、有逻辑的文本，可以用于创作诗歌、小说等文学作品。
                        文章生成：如果需要生成长文本文章，可以使用TextDavinciV3模型。该模型可以根据给定的主题和关键词生成符合语法和逻辑的文章。
                        自动摘要：使用TextDavinciV3模型可以生成摘要，摘要内容可以是一篇文章或多篇文章的主题、重点和关键字，这些信息可以帮助读者更好地理解文章内容。
                        语音助手：可以使用TextDavinciV3模型作为语音助手的后端，为用户提供问答、自动补全、文章摘要等功能。
                    总之，TextDavinciV3模型适用于多种文本生成场景，可以根据具体的应用需求选择合适的API接口和模型。
                 */
                opt.DefaultModelId = Models.TextDavinciV3;
            });

            context.Services.AddScoped<IOpenAIManager, OpenAIManager>();
        }
        else
        {
            context.Services.AddScoped<IOpenAIManager, NullOpenAIManager>();
        }
    }
}