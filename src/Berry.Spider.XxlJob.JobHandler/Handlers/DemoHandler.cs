using DotXxlJob.Core;
using DotXxlJob.Core.Model;

namespace Berry.Spider.XxlJob.JobHandler.Handlers;

[JobHandler("demoJobHandler")]
public class DemoHandler : AbstractJobHandler
{
    public override Task<ReturnT> Execute(JobExecuteContext context)
    {
        context.JobLogger.Log("receive demo job handler,parameter:{0}", context.JobParameter);

        return Task.FromResult(ReturnT.SUCCESS);
    }
}