using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Hangfire
{
    internal class HangfireConst
    {
        /// <summary>
        /// Hangfire-管理鉴权Token
        /// </summary>
        public const string TokenKey = "TokenCookieKey4HangfireAuth";

        /// <summary>
        /// Hangfire-密码输入关键词
        /// </summary>
        public const string PasswordQuery = "pwd";

        /// <summary>
        /// Hangfire-密码
        /// </summary>
        public static string Password;
    }
}
