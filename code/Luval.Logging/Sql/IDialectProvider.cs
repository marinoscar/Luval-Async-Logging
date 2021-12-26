using Luval.Logging.Entities;

namespace Luval.Logging.Sql
{
    public interface IDialectProvider
    {
        string ToSqlInsert(LogMessage logMessage);
    }
}