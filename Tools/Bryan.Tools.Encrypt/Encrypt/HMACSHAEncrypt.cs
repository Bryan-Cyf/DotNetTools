using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tools.Encrypt
{
    public class HMACSHAEncrypt
    {
        public static string HMACSHA1(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA1 hmac = new HMACSHA1(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        public static string HMACSHA256(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        public static byte[] HMACSHA256Bytes(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                return bytes_hamc_out;
            }
        }

        public static string HMACSHA384(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA384 hmac = new HMACSHA384(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);


                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        public static string HMACSHA512(string srcString, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA512 hmac = new HMACSHA512(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }
    }

    public static class HMACSHAExtension
    {
        public static string ToHMACSHA1(this string srcString, string key)
        {
            return HMACSHAEncrypt.HMACSHA1(srcString, key);
        }

        public static string ToHMACSHA256(this string srcString, string key)
        {
            return HMACSHAEncrypt.HMACSHA256(srcString, key);
        }

        public static byte[] ToHMACSHA256Bytes(this string srcString, string key)
        {
            return HMACSHAEncrypt.HMACSHA256Bytes(srcString, key);
        }

        public static string ToHMACSHA384(this string srcString, string key)
        {
            return HMACSHAEncrypt.HMACSHA384(srcString, key);
        }

        public static string ToHMACSHA512(this string srcString, string key)
        {
            return HMACSHAEncrypt.HMACSHA512(srcString, key);
        }
    }
}
