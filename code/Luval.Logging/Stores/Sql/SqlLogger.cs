using Luval.Logging.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Logging.Stores.Sql
{
    /// <summary>
    /// Writes logs messages to a <see cref="IDbConnection"/> data store
    /// </summary>
    public class SqlLogger : ILoggingStore
    {
        private readonly Func<IDbConnection> _createConnection;
        private readonly IDialectProvider _dialectProvider;


        /// <summary>
        /// Initialize an instance of <see cref="SqlLogger"/>
        /// </summary>
        /// <param name="createConnection">A function reference that will createa a new instance of <see cref="IDbConnection"/></param>
        /// <param name="dialectProvider">The <see cref="IDialectProvider" used to create the proper sql statement/></param>
        public SqlLogger(Func<IDbConnection> createConnection, IDialectProvider dialectProvider)
        {
            if (createConnection == null) throw new ArgumentNullException(nameof(createConnection));
            if (dialectProvider == null) throw new ArgumentNullException(nameof(dialectProvider));

            _createConnection = createConnection;
            _dialectProvider = dialectProvider;
        }

        /// <summary>
        /// Persists the <see cref="LogMessage"/> into the database
        /// </summary>
        /// <param name="logMessage">The instance of the <see cref="LogMessage"/> to persist</param>
        /// <param name="cancelationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
        /// <returns>A <see cref="Task"/> that represents the running operation</returns>
        public Task PersistAsync(LogMessage logMessage, CancellationToken cancelationToken)
        {
            return PersistAsync(logMessage, IsolationLevel.ReadCommitted, cancelationToken);
        }

        /// <summary>
        /// Persists the <see cref="LogMessage"/> into the database
        /// </summary>
        /// <param name="logMessage">The instance of the <see cref="LogMessage"/> to persist</param>
        /// <param name="isolationLevel">One of the <see cref="IsolationLevel"/> values</param>
        /// <param name="cancelationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
        /// <returns>A <see cref="Task"/> that represents the running operation</returns>
        public Task PersistAsync(LogMessage logMessage, IsolationLevel isolationLevel, CancellationToken cancelationToken)
        {
            return Task.Run(() =>
            {
                Persist(logMessage, isolationLevel);
            }, cancelationToken);
        }

        /// <summary>
        /// Persists the <see cref="LogMessage"/> into the database
        /// </summary>
        /// <param name="logMessage">The instance of the <see cref="LogMessage"/> to persist</param>
        public void Persist(LogMessage logMessage)
        {
            Persist(logMessage, IsolationLevel.ReadCommitted);
        }


        /// <summary>
        /// Persists the LogMessage into the database
        /// </summary>
        /// <param name="logMessage">The instance of the <see cref="LogMessage"/> to persist</param>
        /// <param name="isolationLevel">One of the <see cref="IsolationLevel"/> values</param>
        public void Persist(LogMessage logMessage, IsolationLevel isolationLevel)
        {
            ExecuteCommand(_dialectProvider.ToSqlInsert(logMessage), isolationLevel);
        }


        /// <summary>
        /// Purgers log messages from the data store
        /// </summary>
        /// <param name="logRetentionInHours">The number of hours of logs to keep in the store</param>
        /// <returns>The number of affected records</returns>
        public int PurgeLogs(int logRetentionInHours)
        {
            var dt = DateTime.UtcNow.AddHours(logRetentionInHours * -1);
            return ExecuteCommand(_dialectProvider.ToSqlDeleteByTimestamp(dt), IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Purgers log messages from the data store
        /// </summary>
        /// <param name="logRetentionInHours">The number of hours of logs to keep in the store</param>
        /// <param name="cancelationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
        /// <returns>A <see cref="Task"/> with the operation for the number of affected records</returns>
        public Task<int> PurgeLogsAsync(int logRetentionInHours, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return PurgeLogs(logRetentionInHours); }, cancellationToken);
        }

        private int ExecuteCommand(string sqlCmd, IsolationLevel isolationLevel)
        {
            var result = 0;
            using (var cnn = _createConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    OpenConnection(cnn);
                    using (var tran = cnn.BeginTransaction(isolationLevel))
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = sqlCmd;
                        cmd.CommandTimeout = cnn.ConnectionTimeout;
                        try
                        {
                            result = cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            Debug.WriteLine(ex.ToString());
                            throw new Exception("Unable to execute the sql command", ex);
                        }
                        finally
                        {
                            CloseConnection(cnn);
                        }
                    }
                }
            }
            return result;
        }

        private void OpenConnection(IDbConnection databaseConnection)
        {
            if (databaseConnection == null) throw new ArgumentNullException(nameof(databaseConnection));
            if (string.IsNullOrWhiteSpace(databaseConnection.ConnectionString)) throw new ArgumentNullException("ConnectionString");
            if (databaseConnection.State == ConnectionState.Closed)
                databaseConnection.Open();
        }

        private void CloseConnection(IDbConnection cnn)
        {
            if (cnn.State != ConnectionState.Closed)
                cnn.Close();
        }
    }
}
