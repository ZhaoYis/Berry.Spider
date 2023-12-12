using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
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
            if (!context.ActionDescriptor.IsControllerAction() || !IsEnableDataWrapper(context))
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

                objectResult.DeclaredType = typeof(ApiResponse);
            }

            await next();
        }

        private bool IsEnableDataWrapper(ResultExecutingContext context)
        {
            if (context.Controller.GetType().IsDefined(typeof(EnableDataWrapperAttribute)))
            {
                return !context.ActionDescriptor.GetMethodInfo().IsDefined(typeof(DisableDataWrapperAttribute));
            }
            else
            {
                return context.ActionDescriptor.GetMethodInfo().IsDefined(typeof(EnableDataWrapperAttribute));
            }
        }
    }
}