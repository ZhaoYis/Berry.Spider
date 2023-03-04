using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider.Consumers.HttpApi;

[Route("cap/api")]
public class HealthCheckController
{
    [Route("health")]
    public string Check()
    {
        return "ok";
    }
}