namespace Berry.Spider.Weixin;

public class WeixinResult
{
    public int errcode { get; set; }

    public string errmsg { get; set; }

    public bool IsSuccessful => this.errcode == 0;
}