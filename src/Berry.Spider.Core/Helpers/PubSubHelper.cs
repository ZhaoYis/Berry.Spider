using System.Collections.Concurrent;

namespace Berry.Spider.Core;

public class PubSubHelper<T> where T : notnull
{
    private readonly HashSet<string> subscribers = new HashSet<string>();
    private readonly ConcurrentQueue<T?> messageQueue = new ConcurrentQueue<T?>();
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// 发布消息的方法
    /// </summary>
    public void Publish(T? message)
    {
        messageQueue.Enqueue(message);
    }

    /// <summary>
    /// 订阅方法，订阅者从队列中取消息
    /// </summary>
    public void Subscribe(string subscriberId, Func<T?, Task> exector)
    {
        // 检查订阅者是否已订阅
        if (!subscribers.Add(subscriberId))
        {
            Console.WriteLine($"Subscriber {subscriberId} is already subscribed.");
            return;
        }

        Task.Run(() =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                // 尝试从队列中获取消息
                if (messageQueue.TryDequeue(out T? message))
                {
                    exector.Invoke(message);
                }
                else
                {
                    // 如果队列为空，休眠一会儿再继续尝试
                    Thread.Sleep(100);
                }
            }
        });
    }

    /// <summary>
    /// 停止所有订阅者
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}