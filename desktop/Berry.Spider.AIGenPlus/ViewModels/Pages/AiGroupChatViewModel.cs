using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGenPlus.ViewModels.Pages;

/// <summary>
/// https://learn.microsoft.com/zh-cn/semantic-kernel/support/archive/agent-chat?pivots=programming-language-csharp
/// </summary>
/// <param name="kernel"></param>
public partial class AiGroupChatViewModel(Kernel kernel) : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 问题
    /// </summary>
    [ObservableProperty] private string _askAiRequestText = null!;

    /// <summary>
    /// AI回答的内容
    /// </summary>
    [ObservableProperty] private string _askAiResponseText = null!;

    const string ReviewerName = "Reviewer";
    const string WriterName = "Writer";

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task AiGroupChatAsync()
    {
        Check.NotNullOrWhiteSpace(this.AskAiRequestText, nameof(this.AskAiRequestText));
        this.ShowNotificationMessage("请稍后，AI正在努力思考中...");

        ChatCompletionAgent agentReviewer = new()
        {
            Name = ReviewerName,
            Instructions =
                """
                Your responsibility is to review and identify how to improve user provided content.
                If the user has providing input or direction for content already provided, specify how to address this input.
                Never directly perform the correction or provide example.
                Once the content has been updated in a subsequent response, you will review the content again until satisfactory.
                Always copy satisfactory content to the clipboard using available tools and inform user.
                RULES:
                - Only identify suggestions that are specific and actionable.
                - Verify previous suggestions have been addressed.
                - Never repeat previous suggestions.
                """,
            Kernel = kernel,
            Arguments = new KernelArguments(new OllamaPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };

        ChatCompletionAgent agentWriter = new()
        {
            Name = WriterName,
            Instructions =
                """
                Your sole responsibility is to rewrite content according to review suggestions.
                - Always apply all review direction.
                - Always revise the content in its entirety without explanation.
                - Never address the user.
                """,
            Kernel = kernel,
        };

        KernelFunction selectionFunction = CreatePromptFunctionForStrategy(
                                                                           $$$"""
                                                                              Examine the provided RESPONSE and choose the next participant.
                                                                              State only the name of the chosen participant without explanation.
                                                                              Never choose the participant named in the RESPONSE.
                                                                              Choose only from these participants:
                                                                              - {{{ReviewerName}}}
                                                                              - {{{WriterName}}}
                                                                              Always follow these rules when choosing the next participant:
                                                                              - If RESPONSE is user input, it is {{{ReviewerName}}}'s turn.
                                                                              - If RESPONSE is by {{{ReviewerName}}}, it is {{{WriterName}}}'s turn.
                                                                              - If RESPONSE is by {{{WriterName}}}, it is {{{ReviewerName}}}'s turn.
                                                                              RESPONSE:
                                                                              {{${{{KernelFunctionTerminationStrategy.DefaultHistoryVariableName}}}}}
                                                                              检查所提供的 RESPONSE 并选择下一位参与者。
                                                                              只说出所选参与者的姓名，不作任何解释。
                                                                              切勿选择 RESPONSE 中列出的参与者。
                                                                              """
                                                                          );

        const string TerminationToken = "yes";
        KernelFunction terminationFunction = CreatePromptFunctionForStrategy(
                                                                             $$$"""
                                                                                Examine the RESPONSE and determine whether the content has been deemed satisfactory.
                                                                                If content is satisfactory, respond with a single word without explanation: {{{TerminationToken}}}.
                                                                                If specific suggestions are being provided, it is not satisfactory.
                                                                                If no correction is suggested, it is satisfactory.
                                                                                RESPONSE:
                                                                                {{${{{KernelFunctionTerminationStrategy.DefaultHistoryVariableName}}}}}
                                                                                """
                                                                            );
        ChatHistoryTruncationReducer historyReducer = new(1);
        AgentGroupChat chat = new(agentReviewer, agentWriter)
        {
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                {
                    // Always start with the editor agent.
                    InitialAgent = agentReviewer,
                    // Save tokens by only including the final response
                    HistoryReducer = historyReducer,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = KernelFunctionTerminationStrategy.DefaultHistoryVariableName,
                    // Returns the entire result value as a string.
                    ResultParser = (result) => result.GetValue<string>() ?? agentReviewer.Name
                },
                TerminationStrategy = new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                {
                    // Only evaluate for editor's response
                    Agents = [agentReviewer],
                    // Save tokens by only including the final response
                    HistoryReducer = historyReducer,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = KernelFunctionTerminationStrategy.DefaultHistoryVariableName,
                    // Limit total number of turns
                    MaximumIterations = 12,
                    // Customer result parser to determine if the response is "yes"
                    ResultParser = (result) => result.GetValue<string>()?.Contains(TerminationToken, StringComparison.OrdinalIgnoreCase) ?? false
                }
            }
        };

        bool isComplete = false;
        StringBuilder aiResponseStringBuilder = new StringBuilder();
        do
        {
            string input = this.AskAiRequestText.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            aiResponseStringBuilder.Append("User >").Append(Environment.NewLine).Append(input);
            aiResponseStringBuilder.Append(Environment.NewLine);
            this.AskAiResponseText = aiResponseStringBuilder.ToString();

            if (input.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                isComplete = true;
                break;
            }

            if (input.Equals("RESET", StringComparison.OrdinalIgnoreCase))
            {
                await chat.ResetAsync();
                Console.WriteLine("[Conversation has been reset]");
                continue;
            }

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));
            chat.IsComplete = false;
            this.AskAiRequestText = string.Empty;
            try
            {
                // await foreach (StreamingChatMessageContent response in chat.InvokeStreamingAsync())
                // {
                //     if (string.IsNullOrWhiteSpace(response?.Content))
                //     {
                //         continue;
                //     }
                //
                //     aiResponseStringBuilder.Append(Environment.NewLine);
                //     this.AskAiResponseText = aiResponseStringBuilder.Append($"{response?.AuthorName?.ToUpperInvariant()}({response?.Role.ToString()}):{Environment.NewLine}{response?.Content}").ToString();
                //     aiResponseStringBuilder.Append(Environment.NewLine);
                // }

                await foreach (ChatMessageContent response in chat.InvokeAsync())
                {
                    if (string.IsNullOrWhiteSpace(response?.Content))
                    {
                        continue;
                    }

                    aiResponseStringBuilder.Append(Environment.NewLine);
                    this.AskAiResponseText = aiResponseStringBuilder.Append($"{response?.AuthorName?.ToUpperInvariant()}({response?.Role.ToString()}):{Environment.NewLine}{response?.Content}").ToString();
                    aiResponseStringBuilder.Append(Environment.NewLine);
                }
            }
            catch (HttpOperationException exception)
            {
                Console.WriteLine(exception.Message);
                if (exception.InnerException != null)
                {
                    Console.WriteLine(exception.InnerException.Message);
                    if (exception.InnerException.Data.Count > 0)
                    {
                        Console.WriteLine(JsonSerializer.Serialize(exception.InnerException.Data, new JsonSerializerOptions()
                        {
                            WriteIndented = true
                        }));
                    }
                }
            }
        } while (!isComplete);
    }

    private bool CanExecute()
    {
        return true;
    }

    public static KernelFunction CreatePromptFunctionForStrategy(string template, IPromptTemplateFactory? templateFactory = null, params string[] safeParameterNames)
    {
        PromptTemplateConfig config = new(template)
        {
            InputVariables = safeParameterNames.Select(parameterName => new InputVariable
            {
                Name = parameterName,
                AllowDangerouslySetContent = true
            }).ToList()
        };

        return KernelFunctionFactory.CreateFromPrompt(config, promptTemplateFactory: templateFactory);
    }
}