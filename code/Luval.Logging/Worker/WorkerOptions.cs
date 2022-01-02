using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Worker
{
    /// <summary>
    /// Provides the options for the <see cref="EventHandlerLoggerWorker"/>
    /// </summary>
    public class WorkerOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="WorkerOptions"/>
        /// </summary>
        public WorkerOptions()
        {
            StartTime = null;
            Interval = TimeSpan.FromSeconds(15);
            MaxMessagedPerCycle = 60;
            LogRetentionInHours = 168;
        }

        /// <summary>
        /// A <see cref="DateTimeOffset"/> of when the worker should start, if null, the worker starts immediately 
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }
        /// <summary>
        /// The time interval between invocations of <see cref="EventHandlerLoggerWorker"/> defaults to 15 seconds
        /// </summary>
        public TimeSpan Interval { get; set; }
        /// <summary>
        /// Indetify the max number of messages that would be processed per interval cycle, defaults to 60
        /// </summary>
        public int MaxMessagedPerCycle { get; set; }
        /// <summary>
        /// Identifies the number of hours the logs will be persited in the store, defaults to 168
        /// </summary>
        public int LogRetentionInHours { get; set; }
    }
}
