namespace Berry.Spider.HttpApi.Host;

internal static class HostEnvironmentEnvExtensions
{
    public static bool IsDev(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment("DEV");
    }

    public static bool IsProd(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment("PROD");
    }
}