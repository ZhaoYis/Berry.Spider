using Microsoft.AspNetCore.Mvc.Filters;

namespace Berry.Spider.AspNetCore.Mvc;

public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        throw new NotImplementedException();
    }
}