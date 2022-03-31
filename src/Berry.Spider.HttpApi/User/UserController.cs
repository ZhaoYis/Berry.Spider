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
    public Task<UserLoginDto> LoginAsync([FromBody] UserLoginInput input)
    {
        return Task.FromResult(new UserLoginDto
        {
            UserId = "20134421",
            UserName = "大师兄丶",
            RealName = "大师兄の真实姓名",
            Desc = "",
            Token = "20134421",
            Role = new List<UserRoleDto>
            {
                new UserRoleDto { RoleName = "Super Admin", Value = "sa" }
            }
        });
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("logout")]
    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("getUserInfo")]
    public Task<UserInfoDto> GetUserInfoAsync()
    {
        return Task.FromResult(new UserInfoDto
        {
            UserId = "20134421",
            UserName = "大师兄丶",
            RealName = "大师兄の真实姓名",
            Avatar = "https://q1.qlogo.cn/g?b=qq&nk=190848757&s=640",
            Desc = "",
            Token = "20134421",
            HomePath = "/dashboard/analysis",
            Role = new List<UserRoleDto>
            {
                new UserRoleDto { RoleName = "Super Admin", Value = "sa" }
            }
        });
    }

    /// <summary>
    /// 获取权限
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("getPermCode")]
    public Task GetPermCodeAsync()
    {
        return Task.CompletedTask;
    }
}