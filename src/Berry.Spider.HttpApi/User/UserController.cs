using Berry.Spider.User;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 用户服务
/// </summary>
[Route("api/services/user")]
public class UserController : SpiderControllerBase
{
    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost, Route("login")]
    public async Task<UserLoginDto> LoginAsync([FromBody] UserLoginInput input)
    {
        return await Task.FromResult(new UserLoginDto()
        { Name = "dsx", Avatar = "", Token = this.Clock.Now.ToString("d") });
    }
}