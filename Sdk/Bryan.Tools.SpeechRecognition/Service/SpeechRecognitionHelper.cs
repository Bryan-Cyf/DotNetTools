using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tools.SpeechRecognition
{
    internal class SpeechRecognitionHelper
    {
        /// <summary>
        /// 有序字典转Url参数
        /// </summary>
        /// <param name="dics"></param>
        /// <returns></returns>
        public static string ToUrl(SortedDictionary<string, object> dics)
        {
            string str = "";
            foreach (var dic in dics)
            {
                str += dic.Key + "=" + dic.Value + "&";
            }
            //去掉最后一个&
            str = str.TrimEnd('&');
            return str;
        }

        /// <summary>
        /// hmac sha1签名
        /// </summary>
        /// <param name="str">要签名的字符串</param>
        /// <param name="accessKeySecret"></param>
        /// <returns></returns>
        public static string GetHmacSha1(string str, string accessKeySecret)
        {
            //sha1签名
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(accessKeySecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
            var signature = Convert.ToBase64String(hashBytes);

            return signature;
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式10位(s)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
