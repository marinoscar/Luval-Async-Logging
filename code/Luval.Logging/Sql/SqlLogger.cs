using Luval.Logging.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Logging.Sql
{
    /// <summary>
    /// Writes logs messages with a IDbConnection
    /// </summary>
    public class SqlLogger : IDisposable, ISlowLogger
    {
        private readonly IDbConnection _connection;
        private readonly IDialectProvider _dialectProvider;


        /// <summary>
        /// Initialize an instance of <see cref="SqlLogger"/>
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" instance to use to write to the database/></param>
        /// <param name="dialectProvider">The <see cref="IDialectProvider" used to create the proper sql statement/></param>
        public SqlLogger(IDbConnection connection, IDialectProvider dialectProvider)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (dialectProvider == null) throw new ArgumentNullException(nameof(dialectProvider));
            if (string.IsNullOrWhiteSpace(connection.ConnectionString)) throw new ArgumentException("connection requires a connection string");

            _connection = connection;
            _dialectProvider = dialectProvider;
        }

        /// <summary>
        /// Persists the <see cref="LogMessage"/> into the database
        /// </summary>
        /// <param name="logMessage">The instance of the <see cref="LogMessage"/> to persist</param>
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

        private void ExecuteCommand(string sqlCmd, IsolationLevel isolationLevel)
        {
            using (var cmd = _connection.CreateCommand())
            {
                using (var tran = _connection.BeginTransaction(isolationLevel))
                {
                    cmd.CommandText = sqlCmd;
                    cmd.CommandTimeout = _connection.ConnectionTimeout;
                    OpenConnection();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception("Unable to execute the sql command", ex);
                    }
                    finally
                    {
                        CloseConnection();
                    }
                }
            }
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
        }

        private void CloseConnection()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }
    }
}
