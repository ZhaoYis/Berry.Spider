using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Berry.Spider.Core;
using MediatR;

namespace Berry.Spider.ToolkitStore.ViewModels.Pages;

public partial class TouTiaoViewModel
{
    private static readonly PubSubHelper<ChildPageTitleRequest> _pubSubHelper = new PubSubHelper<ChildPageTitleRequest>();

    public readonly struct ChildPageTitleRequest(string savaFilePath, List<string> titles) : IRequest
    {
        public string SaveFilePath { get; } = savaFilePath;

        public List<string> Titles { get; } = titles;
    }

    public class ChildPageTitleHandler : IRequestHandler<ChildPageTitleRequest>
    {
        public ChildPageTitleHandler()
        {
            _pubSubHelper.Subscribe(nameof(ChildPageTitleHandler), async (request) =>
            {
                Debug.WriteLine("执行保存中...");
                await FileHelper.WriteToFileAsync(request.SaveFilePath, string.Join(Environment.NewLine, request.Titles));
            });
        }

        public Task Handle(ChildPageTitleRequest request, CancellationToken cancellationToken)
        {
            if (request.Titles is { Count: > 0 })
            {
                _pubSubHelper.Publish(request);
            }

            return Task.CompletedTask;
        }
    }
}