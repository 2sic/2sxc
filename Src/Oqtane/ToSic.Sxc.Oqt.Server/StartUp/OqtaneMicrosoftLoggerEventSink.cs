using Microsoft.Extensions.Logging;

namespace ToSic.Sxc.Oqt.Server.StartUp;

internal sealed class OqtaneMicrosoftLoggerEventSink(ILoggerFactory loggerFactory) : ILogEventSink
{
    private static readonly EventId EntryEvent = new(1, "2sxc.Entry");
    private static readonly EventId CompletionEvent = new(2, "2sxc.Completion");

    private readonly ILogger _logger = loggerFactory.CreateLogger("ToSic.2sxc");

    public void Write(ILog log, Entry entry)
    {
        var level = GetLevel(entry.Message);
        if (!_logger.IsEnabled(level))
            return;

        if (entry.WrapOpenWasClosed)
        {
            _logger.Log(level, CompletionEvent,
                "{SxcSource} completed {SxcMessage} in {SxcElapsedMilliseconds} ms: {SxcResult}",
                entry.Source, entry.Message, entry.Elapsed.TotalMilliseconds, entry.Result);
            return;
        }

        _logger.Log(level, EntryEvent, "{SxcSource} depth {SxcDepth}: {SxcMessage}",
            entry.Source, entry.Depth, entry.Message);
    }

    private static LogLevel GetLevel(string message)
        => message?.StartsWith(LogConstants.ErrorPrefix, StringComparison.Ordinal) == true
            ? LogLevel.Error
            : message?.StartsWith(LogConstants.WarningPrefix, StringComparison.Ordinal) == true
                ? LogLevel.Warning
                : LogLevel.Trace;
}
