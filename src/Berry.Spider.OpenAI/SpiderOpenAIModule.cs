using Berry.Spider.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3.Extensions;
using OpenAI.GPT3.ObjectModels;
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
                //参考https://platform.openai.com/docs/models/overview
                opt.DefaultModelId = Models.Davinci;
            });

            context.Services.AddScoped<IOpenAIManager, OpenAIManager>();
        }
    }
}