using Microsoft.Extensions.Logging;
using DnnLog = DotNetNuke.Instrumentation.ILog;

namespace ToSic.Sxc.Dnn.StartUp;

// Fallback for DNN < 10.4.0: adapts Microsoft ILogger to DNN's legacy log4net-backed ILog.
internal sealed class DnnLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new DnnLogger(categoryName);

    public void Dispose()
    { }

    private sealed class DnnLogger(string categoryName) : ILogger
    {
        private readonly DnnLog _log = DotNetNuke.Instrumentation.LoggerSource.Instance.GetLogger(categoryName);

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => _log.IsTraceEnabled,
            LogLevel.Debug => _log.IsDebugEnabled,
            LogLevel.Information => _log.IsInfoEnabled,
            LogLevel.Warning => _log.IsWarnEnabled,
            LogLevel.Error => _log.IsErrorEnabled,
            LogLevel.Critical => _log.IsFatalEnabled,
            _ => false,
        };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _log.Trace(message, exception);
                    break;
                case LogLevel.Debug:
                    _log.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    _log.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    _log.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _log.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    _log.Fatal(message, exception);
                    break;
            }
        }
    }
}
