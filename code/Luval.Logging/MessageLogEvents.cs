using Luval.Logging.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging
{
    /// <summary>
    /// Class that provides a way to handle all of the message events of all of the <see cref="EventHandlerLogger"/> instances
    /// </summary>
    public class MessageLogEvents
    {
        private static MessageLogEvents _instance;

        /// <summary>
        /// Singleton implementation of <see cref="MessageLogEvents"/>
        /// </summary>
        public static MessageLogEvents Instance
        {
            get
            {
                return _instance ??= new MessageLogEvents();
            }
        }

        private MessageLogEvents() { }

        /// <summary>
        /// Triggers an event every time a <see cref="EventHandlerLogger"/> instance creates a new message
        /// </summary>
        public event EventHandler<LogEventArgs> MessageLogged;

        private void OnMessageLogged(object sender, LogEventArgs e)
        {
            MessageLogged?.Invoke(sender, e);
        }

        internal void DoLogMessage(object sender, LogMessage logMessage)
        {
            OnMessageLogged(sender, new LogEventArgs(logMessage));
        }


    }

    /// <summary>
    /// Provdes the event data for the <see cref="EventHandlerLogger" class/>
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
