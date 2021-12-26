﻿using Luval.Logging.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging
{
    /// <summary>
    /// Provides an implementation of the <see cref="ILogger"/> interface that creates events every time the <seealso cref="Log{TState}(LogLevel, EventId, TState, Exception, Func{TState, Exception, string})" method is invoked/>
    /// </summary>
    public class EventLogger : ILogger
    {
        protected virtual string CategoryName { get; private set; }
        protected virtual IExternalScopeProvider ScopeProvider { get; private set; }
        protected virtual Func<string, LogLevel, bool> Filter { get; private set; }

        /// <summary>
        /// Initialize an instance of <see cref="EventLogger"/>
        /// </summary>
        /// <param name="categoryName">Category name for the logger</param>
        /// <param name="levelFilter">Indicates if any particular <see cref="LogLevel"/> should be filtered</param>
        /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" to use/></param>
        public EventLogger(string categoryName, Func<string, LogLevel, bool> levelFilter, IExternalScopeProvider scopeProvider)
        {
            CategoryName = categoryName; ScopeProvider = scopeProvider; Filter = levelFilter;
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? EmptyScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            if (Filter == null) return true;
            if (logLevel == LogLevel.None)
                return false;
            return Filter(CategoryName, logLevel);
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <see cref="string"/> message of the state and exception</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            OnMessageLogged(new LogEventArgs(LogMessage.Create(CategoryName,
                logLevel, eventId, exception, formatter(state, exception))));
        }

        public event EventHandler<LogEventArgs> MessageLogged;

        protected virtual void OnMessageLogged(LogEventArgs e)
        {
            MessageLogged?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Provdes the event data for the <see cref="EventLogger" class/>
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LogEventArgs"/>
        /// </summary>
        /// <param name="logMessage">The <see cref="LogMessage"/> to pass as the event data</param>
        public LogEventArgs(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }

        public LogMessage LogMessage { get; private set; }
    }

    internal class EmptyScope : IExternalScopeProvider, IDisposable
    {
        public static EmptyScope Instance { get; } = new EmptyScope();

        private EmptyScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        public IDisposable Push(object state)
        {
            return null;
        }
    }
}