using Luval.Logging.Entities;

namespace Luval.Logging.Stores.Sql
{
    public interface IDialectProvider
    {
        string ToSqlInsert(LogMessage logMessage);
    }
}