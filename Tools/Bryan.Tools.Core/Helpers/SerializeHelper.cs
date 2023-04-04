using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Tools
{
    public class SerializeHelper
    {
        public static byte[] ByteSerialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
        public static T ByteDerialize<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

    }

    public static class SerializeHelperExt
    {
        public static byte[] ByteSerialize<T>(this T obj)
        {
            return Encoding.UTF8.GetBytes(obj is string ? obj as string : JsonConvert.SerializeObject(obj));
        }
        public static string ByteDerialize(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string JsonSerialize<T>(this T input)
        {
            return JsonConvert.SerializeObject(input);
        }
        public static T JsonDerialize<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
