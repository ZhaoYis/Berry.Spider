using Serilog.Core;
using Serilog.Events;

namespace Berry.Spider.Consumers;

public class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ThreadId", Thread.CurrentThread.ManagedThreadId));
    }
}