using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Berry.Spider.HttpApi.Host.Controllers;

[Authorize]
[Route("api/claims")]
public class ClaimsController : AbpController
{
    [HttpGet]
    public JsonResult Get()
    {
        return Json(User.Claims.Select(x => new { Type = x.Type, Value = x.Value }));
    }
}