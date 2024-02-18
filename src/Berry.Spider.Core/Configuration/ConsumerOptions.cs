namespace Berry.Spider.Core;

public class ConsumerOptions
{
    /// <summary>
    /// The number of consumer thread connections.
    /// Default is 1
    /// </summary>
    public int ConsumerThreadCount { get; set; }

    /// <summary>
    /// If true, the message will be pre fetch to memory queue for parallel execute by thread pool.
    /// Default is false
    /// </summary>
    public bool EnableConsumerPrefetch { get; set; }

    /// <summary>
    /// Sent or received succeed message after time span of due, then the message will be deleted at due time.
    /// Default is 1*24*3600 seconds.
    /// </summary>
    public int SucceedMessageExpiredAfter { get; set; } = 1 * 24 * 3600;

    /// <summary>
    /// Sent or received failed message after time span of due, then the message will be deleted at due time.
    /// Default is 3*24*3600 seconds.
    /// </summary>
    public int FailedMessageExpiredAfter { get; set; } = 3 * 24 * 3600;
}