namespace Berry.Spider.Contracts;

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
}