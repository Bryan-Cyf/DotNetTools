using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class StringExtension
{
    public static bool IsNullOrEmpty(this string @this)
    {
        return string.IsNullOrEmpty(@this);
    }

    public static bool IsNotNullOrEmpty(this string @this)
    {
        return !string.IsNullOrEmpty(@this);
    }

    private static char[] trimChars = { ' ', '\t', '\r', '\n', '\u200B', '\u200C', '\u200D', '\uFEFF' };
    /// <summary>
    /// 去除空格,零宽字符等
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string Trimz(this string @this)
    {
        return @this.Trim(trimChars);
    }

    public static string ToSaltMd5(this string @this)
    {
        return ToUpperMd5($"|{Convert.ToBase64String(Encoding.UTF8.GetBytes(@this).Reverse().ToArray())}|{ToUpperMd5(@this)}|");
    }

    public static string ToUpperMd5(this string @this)
    {
        using (var md5 = MD5.Create())
        {
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(@this));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "");
        }
    }
}
