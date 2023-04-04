using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Tools
{
    public static class LockerHelper
    {
        /// <summary>
        /// 判断多开 多开后退出程序
        /// </summary>
        /// <param name="callback">多开时调用</param>
        public static void CheckMutex(Action callback)
        {
            Mutex mutex = new Mutex(false, "Global\\" + SysHelper.AppGuid);
            if (!mutex.WaitOne(0, false))
            {
                callback();
                Environment.Exit(0);
            }
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                mutex.Dispose();
            };
        }
    }
}
