namespace Berry.Spider.Core;

public class HumanMachineVerificationOptions
{
    /// <summary>
    /// 人机验证页面主机黑名单地址
    /// </summary>
    public List<string> BlackHosts { get; set; } = new List<string>();

    /// <summary>
    /// 锁定资源时间。单位：秒
    /// </summary>
    public int LockExpiration { get; set; }
}