using Luval.Logging.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Stores.Sql
{
    /// <summary>
    /// Provides an implementation of <see cref="IDialectProvider" for SQL Server databases/>
    /// </summary>
    public class MsSqlDialectProvider : IDialectProvider
    {
        /// <summary>
        /// Creates a sql statement to insert the <see cref="LogMessage" entity/>
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        public string ToSqlInsert(LogMessage logMessage)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO [dbo].[{0}] ", nameof(LogMessage));
            sb.Append("([MachineName], [UtcTimestamp], [MessageType], [Logger], [Message], [Exception]) ");
            sb.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4}, {5}) ",
                ToSql(logMessage.MachineName), ToSql(logMessage.UtcTimestamp), ToSql(logMessage.LogLevel), ToSql(logMessage.Logger), ToSql(logMessage.Message), ToSql(logMessage.Exception));
            return sb.ToString();
        }

        private string ToSql(string s)
        {
            if (string.IsNullOrEmpty(s)) return "NULL";
            return string.Format("'{0}'", s.Replace("'", "''"));
        }

        private string ToSql(DateTime d)
        {
            return string.Format("'{0:yyyy-MM-dd HH:mm:ss.fff}'", d);
        }

        private int ToSql(LogLevel l)
        {
            return Convert.ToInt32(l);
        }
    }
}
