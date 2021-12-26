using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Worker
{
    /// <summary>
    /// Provides the options for the <see cref="LogWorker"/>
    /// </summary>
    public class WorkerOptions
    {

        /// <summary>
        /// A <see cref="DateTimeOffset"/> of when the worker should start, if null, the worker starts immediately 
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }
        /// <summary>
        /// The time interval between invocations of <see cref="LogWorker"/>
        /// </summary>
        public TimeSpan Interval { get; set; }
        /// <summary>
        /// Indetify the max number of messages that would be processed per interval cycle
        /// </summary>
        public int MaxMessagedPerCycle { get; set; }
    }
}
