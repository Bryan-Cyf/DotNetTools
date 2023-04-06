using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Tools.Hangfire
{
    public enum StorageTypeEnum
    {
        /// <summary>
        /// 内存
        /// </summary>
        [Description("内存")]
        Memory = 0,

        /// <summary>
        /// PostgreSql
        /// </summary>
        [Description("PostgreSql")]
        PostgreSql = 1,

        /// <summary>
        /// MySql
        /// </summary>
        [Description("MySql")]
        MySql = 2,
    }
}
