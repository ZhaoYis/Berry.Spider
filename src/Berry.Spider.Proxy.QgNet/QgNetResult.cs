namespace Berry.Spider.Proxy.QgNet;

public class QgNetResult<T> where T : class, new()
{
    // Code	    Integer	结果编码:0(成功）-1(失败)
    // TaskID	Integer	任务ID
    // Num	    Integer	申请数量
    // Data	    Array	代理IP数据信息，包含节点IP、端口、失效日期
    // Msg	    String	区域ID，多个用”，”分割;*代表全部

    /**
     *  -1 未知错误
        -10 参数不合法!
        -100 计划不存在或已过期!
        -101 请求数量超过计划通道数!
        -102 没有剩余的可用通道。
     */
    public int Code { get; set; }

    public T Data { get; set; }
    public int Num { get; set; }
    public string TaskID { get; set; }
    public string Msg { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess => this.Code == 0;
}