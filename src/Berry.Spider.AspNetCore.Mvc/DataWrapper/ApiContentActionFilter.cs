using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AspNetCore.Mvc
{
    /// <summary>
    /// 统一API接口响应格式
    /// </summary>
    public class ApiContentActionFilter : IAsyncResultFilter, ITransientDependency
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ActionDescriptor.IsControllerAction())
            {
                await next();
                return;
            }

            if (context.Result is ObjectResult objectResult)
            {
                //var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
                if (objectResult.Value != null)
                {
                    if (objectResult.Value is ProblemDetails problemDetails)
                    {
                        string? failedMsg = "";
                        if (objectResult.Value is ValidationProblemDetails validationProblemDetails)
                        {
                            failedMsg = string.Join("", validationProblemDetails.Errors.SelectMany(s => s.Value));
                        }

                        failedMsg = string.IsNullOrEmpty(failedMsg) ? problemDetails.Title : failedMsg;

                        objectResult.Value = problemDetails.Detail == null
                            ? ApiResponse.Failed()
                            : ApiResponse.Failed(problemDetails.Detail, failedMsg);
                    }
                    else
                    {
                        objectResult.Value = objectResult.Value == null
                            ? ApiResponse.Succeed()
                            : ApiResponse.Succeed(objectResult.Value);
                    }
                }
                else
                {
                    objectResult.Value = ApiResponse.Succeed();
                }
            }

            await next();
        }
    }
}