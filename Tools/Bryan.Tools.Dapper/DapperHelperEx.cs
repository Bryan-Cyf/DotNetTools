using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;


public static class DapperHelperEx
{
    public static void Using(this IDbConnection connection, Action<IDbConnection> action)
    {
        using (connection)
        {
            action(connection);
        }
    }

    public static async Task Using(this IDbConnection connection, Func<IDbConnection, Task> func)
    {
        using (connection)
        {
            await func(connection);
        }
    }
}

