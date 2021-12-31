using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.Stores.Sql
{
    /// <summary>
    /// Writes logs messages to a <see cref="SqlConnection"/> instance
    /// </summary>
    public class MsSqlLogger : SqlLogger
    {
        /// <summary>
        /// Creates a new instance of <see cref="MsSqlLogger"/>
        /// </summary>
        /// <param name="connectionString">The sql connection string</param>
        public MsSqlLogger(string connectionString): base(() => { return new SqlConnection(connectionString); }, new MsSqlDialectProvider())
        {

        }
    }
}
