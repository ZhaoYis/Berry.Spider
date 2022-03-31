namespace Berry.Spider.User;

public class UserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 用户真实名
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// TOKEN
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 首页
    /// </summary>
    public string HomePath { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    public List<UserRoleDto> Role { get; set; }
}