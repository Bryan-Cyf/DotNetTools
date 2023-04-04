using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tools.Encrypt
{
    public class SHAEncrypt
    {
        public static string SHA1(string str)
        {
            using (SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
                string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
                str_sha1_out = str_sha1_out.Replace("-", "");
                return str_sha1_out;
            }
        }

        public static string SHA256(string srcString)
        {
            using (SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes_sha256_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
                string str_sha256_out = BitConverter.ToString(bytes_sha256_out);
                str_sha256_out = str_sha256_out.Replace("-", "");
                return str_sha256_out;
            }
        }

        public static string SHA384(string srcString)
        {
            using (SHA384 sha384 = System.Security.Cryptography.SHA384.Create())
            {
                byte[] bytes_sha384_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_sha384_out = sha384.ComputeHash(bytes_sha384_in);
                string str_sha384_out = BitConverter.ToString(bytes_sha384_out);
                str_sha384_out = str_sha384_out.Replace("-", "");
                return str_sha384_out;
            }

        }

        public static string SHA512(string srcString)
        {
            using (SHA512 sha512 = System.Security.Cryptography.SHA512.Create())
            {
                byte[] bytes_sha512_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_sha512_out = sha512.ComputeHash(bytes_sha512_in);
                string str_sha512_out = BitConverter.ToString(bytes_sha512_out);
                str_sha512_out = str_sha512_out.Replace("-", "");
                return str_sha512_out;
            }
        }
    }

    public static class SHAExtension
    {
        public static string ToSHA1(this string srcString)
        {
            return SHAEncrypt.SHA1(srcString);
        }

        public static string ToSHA256(this string srcString)
        {
            return SHAEncrypt.SHA256(srcString);
        }

        public static string ToSHA384(this string srcString)
        {
            return SHAEncrypt.SHA384(srcString);
        }

        public static string ToSHA512(this string srcString)
        {
            return SHAEncrypt.SHA512(srcString);
        }
    }
}
