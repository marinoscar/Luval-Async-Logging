using Luval.Logging.Entities;
using System;

namespace Luval.Logging.Stores.Sql
{
    public interface IDialectProvider
    {
        string ToSqlInsert(LogMessage logMessage);
        string ToSqlDeleteByTimestamp(DateTime dateTime);
    }
}