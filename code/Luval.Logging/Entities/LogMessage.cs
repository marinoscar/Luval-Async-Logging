using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Entities
{
    /// <summary>
    /// Data structure to represent the log message
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="LogMessage"/>
        /// </summary>
        public LogMessage()
        {
            MachineName = Environment.MachineName;
            UtcTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets a unique Id for the message
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets the Machine Name from the <see cref="Environment"/> class
        /// </summary>
        public string MachineName { get; }
        /// <summary>
        /// Gets or sets the UTC timestamp when the message took place
        /// </summary>
        public DateTime UtcTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Extensions.Logging.LogLevel"/> for the message
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// Gets or sets the originating logger for the message
        /// </summary>
        public string Logger { get; set; }
        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// gets or sets the exception
        /// </summary>
        public string Exception { get; set; }

        internal static LogMessage Create(string loggerName, LogLevel logLevel, EventId eventId, Exception exception, string message)
        {
            return new LogMessage()
            {
                Message = message,
                Exception = exception?.ToString() ?? null,
                Logger = loggerName,
                LogLevel = logLevel,
                UtcTimestamp = DateTime.UtcNow
            };
        }

    }
}
