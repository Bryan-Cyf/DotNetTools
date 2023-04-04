using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tools.Encrypt
{
    public class HMACMDEncrypt
    {
        public static string HMACMD5(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACMD5 md5 = new HMACMD5(secrectKey))
            {
                byte[] bytes_md5_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
                string str_md5_out = BitConverter.ToString(bytes_md5_out);
                str_md5_out = str_md5_out.Replace("-", "");
                return str_md5_out;
            }
        }
    }

    public static class HMACMDExtension
    {
        public static string ToHMACMD5(this string srcString, string key)
        {
            return HMACMDEncrypt.HMACMD5(srcString, key);
        }
    }
}
