namespace Berry.Spider.Core;

public static class DateTimeExtensions
{
    /// <summary>
    /// 转13位时间戳
    /// </summary>
    /// <returns></returns>
    public static long ToTimestamp(this DateTime dateTime)
    {
        DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long) (dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
    }
}