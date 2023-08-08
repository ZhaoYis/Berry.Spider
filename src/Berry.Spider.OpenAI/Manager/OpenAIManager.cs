using Microsoft.Extensions.Logging;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.SharedModels;
using Volo.Abp;

namespace Berry.Spider.OpenAI;

public class OpenAIManager : IOpenAIManager
{
    private readonly IOpenAIService _openAiService;
    private ILogger<OpenAIManager> Logger { get; }

    public OpenAIManager(IOpenAIService openAiService, ILogger<OpenAIManager> logger)
    {
        _openAiService = openAiService;
        Logger = logger;
    }

    public async Task<string?> CreateCompletionAsync(string prompt, int? maxTokens = 2048)
    {
        /**
         *
            {
                "id": "cmpl-6rOA7nMFnr9fnlmfOqaEkTmtcc7ER",
                "object": "text_completion",
                "created": 1678182283,
                "model": "text-davinci-003",
                "choices": [
                    {
                        "text": "\n\nDavinciV3是一种基于深度学习的语音识别模型，它能够以非常快的速度对语音进行识别。它的核心是采用一种称为Transformer的架构，它可以从输入语音中提取出它所需要的信息，然后将其转换成文本。\n\nDavinciV3的Transformer架构使用多个层次的信息抽取模块，以抽取出输入语音中的实体、语义",
                        "index": 0,
                        "logprobs": null,
                        "finish_reason": "length"
                    }
                ],
                "usage": {
                    "prompt_tokens": 20,
                    "completion_tokens": 255,
                    "total_tokens": 275
                }
            }
         */

        CompletionCreateRequest request = new CompletionCreateRequest
        {
            Prompt = prompt,
            Model = Models.TextDavinciV3,
            MaxTokens = maxTokens
        };
        var completionResult = await _openAiService.Completions.CreateCompletion(request);

        if (completionResult.Successful)
        {
            ChoiceResponse? choiceResponse = completionResult.Choices.FirstOrDefault();
            if (choiceResponse is { })
            {
                return choiceResponse.Text;
            }
        }
        else
        {
            if (completionResult.Error == null)
            {
                throw new BusinessException("Unknown Error");
            }

            Logger.LogError($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        }

        return default;
    }
}