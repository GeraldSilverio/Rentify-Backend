using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace Rentify.Backend.Logging.Tests;

internal sealed class TestLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    public IReadOnlyCollection<LogEvent> Events => _events.ToArray();

    public void Emit(LogEvent logEvent)
    {
        _events.Enqueue(logEvent);
    }

    public string RenderAll()
    {
        return string.Join(
            Environment.NewLine,
            Events.Select(logEvent => string.Join(
                " ",
                logEvent.RenderMessage(),
                logEvent.Exception?.ToString(),
                string.Join(" ", logEvent.Properties.Select(property =>
                    $"{property.Key}={property.Value}")))));
    }
}
