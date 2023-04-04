using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Tools.Hangfire
{
    public enum StorageTypeEnum
    {
        [Description("内存")]
        Memory = 0,

        [Description("PGSQL")]
        PostgreSql = 1,
    }
}
