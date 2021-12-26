using Luval.Logging.Entities;
using Luval.Logging.Sql;
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
    public class LogWorker : IHostedService, IDisposable
    {

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly ConcurrentStack<LogMessage> _messages = new ConcurrentStack<LogMessage>();

        private Task _executingTask;
        private readonly EventLogger _eventLogger;
        private readonly ISlowLogger _slowLogger;
        private readonly WorkerOptions _options;
        private Timer _timer = null!;
        private readonly TimeSpan _dueTime;


        /// <summary>
        /// Initailizes an instance of <see cref="LogWorker"/>
        /// </summary>
        /// <param name="eventLogger">The <see cref="EventLogger"/> use to stack the <see cref="LogMessage"/> coming from the <see cref="ILogger"/> implementation</param>
        /// <param name="slowLogger">The <see cref="ISlowLogger"/> implementation that will persists the <see cref="LogMessage"/> comming from the <see cref="ILogger"/></param>
        /// <param name="options">A <see cref="WorkerOptions"/> object with the <see cref="LogWorker"/> configuration options</param>
        public LogWorker(EventLogger eventLogger, ISlowLogger slowLogger, WorkerOptions options)
        {
            if (eventLogger == null) throw new ArgumentNullException(nameof(eventLogger));
            if (slowLogger == null) throw new ArgumentNullException(nameof(slowLogger));
            if (options == null) throw new ArgumentNullException(nameof(options));

            _eventLogger = eventLogger;
            _slowLogger = slowLogger;
            _options = options;

            if (_options.StartTime != null && _options.StartTime.Value.UtcDateTime > DateTime.UtcNow)
                _dueTime = _options.StartTime.Value.UtcDateTime.Subtract(DateTime.UtcNow);
            else _dueTime = TimeSpan.Zero;

            _eventLogger.MessageLogged += MessageRecieved;
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
            throw new NotImplementedException();
        }

        private void DoWork(object? state)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
        }

        private Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Execute(stoppingToken);
            }, stoppingToken);
        }

        private void Execute(CancellationToken cancellationToken)
        {
            var count = 0;
            while (_messages.Count > 0 || count < _options.MaxMessagedPerCycle)
            {
                if (_messages.TryPop(out LogMessage m))
                {
                    _slowLogger.PersistAsync(m, cancellationToken);
                }
                count++;
            }
        }
    }
}
