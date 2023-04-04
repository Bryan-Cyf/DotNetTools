using System;
using System.Data;
using System.Threading.Tasks;

namespace Helpers
{
    public class DapperHelper
    {
        public static void Using(IDbConnection dbConnection, Action<IDbConnection> action)
        {
            using (dbConnection)
            {
                action(dbConnection);
            }
        }

        public static async Task Using(IDbConnection dbConnection, Func<IDbConnection, Task> func)
        {
            using (dbConnection)
            {
                await func(dbConnection);
            }
        }
    }
}
