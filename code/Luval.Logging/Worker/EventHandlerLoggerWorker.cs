using Luval.Logging.Entities;
using Luval.Logging.Stores;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Logging.Worker
{
    /// <summary>
    /// Provides a background worker implementation to write async log messages to slow targets like SQL databases or others
    /// </summary>
    public class EventHandlerLoggerWorker : IHostedService, IDisposable
    {

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly ConcurrentStack<LogMessage> _messages = new ConcurrentStack<LogMessage>();

        private Task _executingTask;
        private readonly EventHandlerLogger _eventLogger;
        private readonly ILoggingStore _loggingStore;
        private readonly WorkerOptions _options;
        private Timer _timer;
        private readonly TimeSpan _dueTime;
        private static bool _subscribed;


        /// <summary>
        /// Initailizes an instance of <see cref="EventHandlerLoggerWorker"/>
        /// </summary>
        /// <param name="loggingStore">The <see cref="ILoggerStore"/> implementation that will persists the <see cref="LogMessage"/> comming from the <see cref="ILogger"/></param>
        /// <param name="options">A <see cref="WorkerOptions"/> object with the <see cref="EventHandlerLoggerWorker"/> configuration options</param>
        /// <remarks>Requires that <see cref="EventHandlerLogger"/> is registered in the host as a singleton</remarks>
        public EventHandlerLoggerWorker(ILoggingStore loggingStore, WorkerOptions options)
        {
            if (loggingStore == null) throw new ArgumentNullException(nameof(loggingStore));
            if (options == null) throw new ArgumentNullException(nameof(options));

            _loggingStore = loggingStore;
            _options = options;

            if (_options.StartTime != null && _options.StartTime.Value.UtcDateTime > DateTime.UtcNow)
                _dueTime = _options.StartTime.Value.UtcDateTime.Subtract(DateTime.UtcNow);
            else _dueTime = TimeSpan.Zero;

            SubscribeToEvents();
        }

        /// <summary>
        /// Subscribes to all of the <see cref="LogMessage"/> events created by all of the <see cref="EventHandlerLogger"/> instances in the 
        /// </summary>
        public void SubscribeToEvents()
        {
            if (!_subscribed)
            {
                MessageLogEvents.Instance.MessageLogged += MessageRecieved;
                _subscribed = true;
            }
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> instance of the operation</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, _stoppingCts, _dueTime, _options.Interval);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> instance of the operation</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _messages.Clear();
            _stoppingCts.Dispose();
        }

        private void MessageRecieved(object sender, LogEventArgs e)
        {
            //push the message into the stack
            _messages.Push(e.LogMessage);
        }

        private void DoWork(object? state)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
        }

        private Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var count = 0;
                while (_messages.Count > 0 && (_options.MaxMessagedPerCycle > 0 && count < _options.MaxMessagedPerCycle))
                {
                    if (_messages.TryPop(out LogMessage m))
                    {
                        //persist the messages async until there are no pending or
                        //the max per cycle is reached
                        _loggingStore.PersistAsync(m, stoppingToken);
                    }
                    count++;
                }
            }, stoppingToken);
        }
    }
}
