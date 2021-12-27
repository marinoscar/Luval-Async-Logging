using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Configuration;

namespace Luval.Logging
{
    public static class EventHandlerLoggerExtensions
    {
        public static ILoggingBuilder AddEventHandler(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EventHandlerLoggerProvider>());
            return builder;
        }

        public static ILoggerFactory AddEventHandler(this ILoggerFactory factory, EventHandlerLoggerOptions options)
        {
            factory.AddProvider(new EventHandlerLoggerProvider(options));
            return factory;
        }

        public static ILoggerFactory AddEventHandler(this ILoggerFactory factory, Func<LogLevel, bool> levelFilter)
        {
            return AddEventHandler(factory, new EventHandlerLoggerOptions()
            {
                LevelFilter = (s, l) => { return levelFilter(l); }
            });
        }

        public static ILoggerFactory AddEventHandler(this ILoggerFactory factory, LogLevel minLevel)
        {
            return AddEventHandler(factory, new EventHandlerLoggerOptions()
            {
                LevelFilter = (s, l) => { return l >= minLevel; }
            });
        }
    }
}
