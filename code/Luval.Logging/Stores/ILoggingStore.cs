using Luval.Logging.Entities;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Logging.Stores
{
    /// <summary>
    /// Persists logging messages
    /// </summary>
    public interface ILoggingStore
    {

        Task PersistAsync(LogMessage logMessage, CancellationToken cancelationToken);
        /// <summary>
        /// Persists an instance <see cref="LogMessage"/> in the data store
        /// </summary>
        /// <param name="logMessage">The <see cref="LogMessage"/> to persist</param>
        /// <param name="isolationLevel">For implementations with <see cref="IDbConnection"/> allows to set the <see cref="IsolationLevel"/> for the transaction</param>
        /// <param name="cancelationToken">The <see cref="CancellationToken"/> for the operation</param>
        /// <returns>A <see cref="Task"/> with the operation</returns>
        Task PersistAsync(LogMessage logMessage, IsolationLevel isolationLevel, CancellationToken cancelationToken);
    }
}