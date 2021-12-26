using Luval.Logging.Entities;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Logging.Sql
{
    public interface ISlowLogger
    {
        Task PersistAsync(LogMessage logMessage, CancellationToken cancelationToken);
        Task PersistAsync(LogMessage logMessage, IsolationLevel isolationLevel, CancellationToken cancelationToken);
    }
}