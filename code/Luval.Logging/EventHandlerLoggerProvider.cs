using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging
{
    /// <summary>
    /// Provides a way to create instances of <see cref="EventHandlerLogger"/>
    /// </summary>
    public class EventHandlerLoggerProvider : ILoggerProvider
    {
        private readonly EventHandlerLoggerOptions _options;
        //private static Dictionary<string, EventHandlerLogger> _loggers = new();
        private static EventHandlerLogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventHandlerLoggerProvider"/>
        /// </summary>
        public EventHandlerLoggerProvider() : this(new EventHandlerLoggerOptions())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EventHandlerLoggerProvider"/>
        /// </summary>
        /// <param name="options">A <see cref="EventHandlerLoggerOptions"/> with the configuration values to create an instance of <see cref="EventHandlerLogger"/></param>
        public EventHandlerLoggerProvider(EventHandlerLoggerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _options = options;
            _logger = new EventHandlerLogger(_options);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EventHandlerLogger"/>
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created</returns>
        public ILogger CreateLogger(string categoryName)
        {
            //if (string.IsNullOrWhiteSpace(categoryName)) categoryName = _options.CategoryName;
            //if (_loggers.ContainsKey(categoryName)) return _loggers[categoryName];

            //_loggers[categoryName] = new EventHandlerLogger(new EventHandlerLoggerOptions() { 
            //    CategoryName = categoryName, LevelFilter = _options.LevelFilter, ScopeProvider = _options.ScopeProvider
            //});

            //return _loggers[categoryName];
            return _logger;
        }

        public void Dispose()
        {
        }
    }
}
