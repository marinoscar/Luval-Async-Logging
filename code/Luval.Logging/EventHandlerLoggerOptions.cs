using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging
{
    /// <summary>
    /// Options for the <see cref="EventHandlerLogger"/>
    /// </summary>
    public class EventHandlerLoggerOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventHandlerLoggerOptions"/>
        /// </summary>
        public EventHandlerLoggerOptions()
        {
            CategoryName = nameof(EventHandlerLogger);
            ScopeProvider = EmptyScope.Instance;
            LevelFilter = ((c, l) =>
            {
                if (l == LogLevel.None) return false;
                if (l == LogLevel.Debug) return false;
                if (l == LogLevel.Trace) return false;
                return true;
            });
        }

        /// <summary>
        /// Gets or sets the <seealso cref="CategoryName"/>
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the <seealso cref="LevelFilter"/> with a function to filter the <see cref="LogLevel"/> that the <see cref="ILogger"/> will process
        /// </summary>
        public Func<string, LogLevel, bool> LevelFilter { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="IExternalScopeProvider"/>
        /// </summary>
        public IExternalScopeProvider ScopeProvider { get; set; }
    }
}
