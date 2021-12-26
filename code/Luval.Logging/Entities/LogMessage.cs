using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Entities
{
    public class LogMessage
    {
        public LogMessage()
        {
            MachineName = Environment.MachineName;
            UtcTimestamp = DateTime.UtcNow;
        }
        public long Id { get; set; }
        public string MachineName { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public int MessageType { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        internal static LogMessage Create(string loggerName, LogLevel logLevel, EventId eventId, Exception exception, string message)
        {
            return new LogMessage()
            {
                Message = message,
                Exception = exception?.ToString() ?? null,
                Logger = loggerName,
                MachineName = Environment.MachineName,
                MessageType = Convert.ToInt32(logLevel),
                UtcTimestamp = DateTime.UtcNow
            };
        }

    }
}
