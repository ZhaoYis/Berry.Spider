using Berry.Spider.AIGenPlus;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationExtensions
{
    public static T GetRequiredService<T>(this App app) where T : notnull
    {
        return app.Services.GetRequiredService<T>();
    }
}