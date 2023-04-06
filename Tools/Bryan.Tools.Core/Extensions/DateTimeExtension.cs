using System;
using System.Collections.Generic;
using System.Text;

public static class DateTimeExtension
{
    private static readonly long _utcTicks = 621355968000000000;
    private static readonly DateTime _utcDateTime = new System.DateTime(1970, 1, 1);
    /// <summary>
    /// Unix时间戳
    /// </summary>
    /// <returns>秒</returns>
    public static long GetUtcSeconds(this DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks - _utcTicks) / 10000000;
    }

    /// <summary>
    /// Unix时间戳
    /// </summary>
    /// <returns>毫秒</returns>
    public static long GetMilliUtc(this DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks - _utcTicks) / 10000;
    }

    /// <summary>
    /// Unix时间戳
    /// </summary>
    /// <returns>秒</returns>
    public static long GetUtc(this DateTimeOffset dateTimeOffset)
    {
        return (dateTimeOffset.UtcTicks - _utcTicks) / 10000000;
    }

    /// <summary>
    /// Unix时间戳
    /// </summary>
    /// <returns>毫秒</returns>
    public static long GetMilliUtc(this DateTimeOffset dateTimeOffset)
    {
        return (dateTimeOffset.UtcTicks - _utcTicks) / 10000;
    }

    public static DateTime UtcToDateTime(this long utc)
    {
        System.DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(_utcDateTime, TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(utc);
        return dt;
    }
    public static DateTime MilliUtcToDateTime(this long utc)
    {
        System.DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(_utcDateTime, TimeZoneInfo.Local);
        DateTime dt = startTime.AddMilliseconds(utc);
        return dt;
    }

    public static DateTimeOffset UtcToDateTimeOffset(this long utc)
    {
        System.DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(_utcDateTime, TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(utc);
        return dt;
    }
    public static DateTimeOffset MilliUtcToDateTimeOffset(this long utc)
    {
        System.DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(_utcDateTime, TimeZoneInfo.Local);
        DateTime dt = startTime.AddMilliseconds(utc);
        return dt;
    }
}
