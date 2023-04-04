using System;
using System.Collections.Generic;
using System.Text;

public static class DoubleExt
{
    /// <summary>
    /// 时间戳(秒)转成北京时间
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static DateTime ToDateTimeBySecond(this double @this)
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return start.AddSeconds(@this).AddHours(8);
    }
}
