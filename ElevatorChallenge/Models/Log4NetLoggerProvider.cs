using log4net;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Models {
    public class Log4NetLoggerProvider : ILoggerProvider {
        private readonly ILog log;

        public Log4NetLoggerProvider() {
            log = LogManager.GetLogger(typeof(Log4NetLoggerProvider));
        }

        public ILogger CreateLogger(string categoryName) {
            return new Log4NetLogger(log);
        }

        public void Dispose() {

        }
    }
}