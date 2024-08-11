using log4net;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Models {
    public class Log4NetLogger : ILogger {
        private readonly ILog log;

        public Log4NetLogger(ILog log) {
            this.log = log;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) {
            return logLevel switch {
                LogLevel.Trace => log.IsDebugEnabled,
                LogLevel.Debug => log.IsDebugEnabled,
                LogLevel.Information => log.IsInfoEnabled,
                LogLevel.Warning => log.IsWarnEnabled,
                LogLevel.Error => log.IsErrorEnabled,
                LogLevel.Critical => log.IsFatalEnabled,
                _ => false
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            var message = formatter(state, exception);

            if (!IsEnabled(logLevel)) {
                return;
            }

            switch (logLevel) {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    log.Debug(message);
                    break;
                case LogLevel.Information:
                    log.Info(message);
                    break;
                case LogLevel.Warning:
                    log.Warn(message);
                    break;
                case LogLevel.Error:
                    log.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    log.Fatal(message, exception);
                    break;
                default:
                    log.Info(message);
                    break;
            }
        }
    }
}