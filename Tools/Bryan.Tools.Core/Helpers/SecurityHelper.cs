using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tools
{
    /// <summary>
    /// 开源加密解密git：https://github.com/myloveCc/NETCore.Encrypt
    /// </summary>
    public class SecurityHelper
    {
        #region aes加解密

        public static string AESEncrypt(string input, string key)
        {
            var encryptKey = Encoding.UTF8.GetBytes(key);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(encryptKey, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor,
                            CryptoStreamMode.Write))

                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result,
                            iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string AESDecrypt(string input, string key)
        {
            var fullCipher = Convert.FromBase64String(input);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var decryptKey = Encoding.UTF8.GetBytes(key);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(decryptKey, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt,
                            decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
        #endregion

        /// <summary>
        /// md5加密,默认大写
        /// </summary>
        /// <param name="input">明文</param>
        /// <returns></returns>
        public static string ToMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        #region c# des加密解密
        private static byte[] keyvi = new byte[] { 0xa1, 0xb2, 0xc3, 0xd4, 0xe5, 0xf6, 0x07, 0xa8 };
        private static string defaultEncryptKey = "CZKJ~!@#";
        /// <summary>
        /// 加密
        /// </summary>
        public static string DesEncrypt(string normalTxt, string EncryptKey = null)
        {
            EncryptKey = EncryptKey ?? defaultEncryptKey;
            var bytes = Encoding.UTF8.GetBytes(normalTxt);
            var key = Encoding.UTF8.GetBytes(EncryptKey.PadLeft(8, '0').Substring(0, 8));
            using (MemoryStream ms = new MemoryStream())
            {
                var encry = new DESCryptoServiceProvider();
                CryptoStream cs = new CryptoStream(ms, encry.CreateEncryptor(key, keyvi), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string DesDecrypt(string securityTxt, string EncryptKey = null)
        {
            try
            {
                EncryptKey = EncryptKey ?? defaultEncryptKey;
                var bytes = Convert.FromBase64String(securityTxt);
                var key = Encoding.UTF8.GetBytes(EncryptKey.PadLeft(8, '0').Substring(0, 8));
                using (MemoryStream ms = new MemoryStream())
                {
                    var descrypt = new DESCryptoServiceProvider();
                    CryptoStream cs = new CryptoStream(ms, descrypt.CreateDecryptor(key, keyvi), CryptoStreamMode.Write);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region java加密解密
        /// <summary>
        /// 加密
        /// </summary>
        public static string DesEncryptWithJava(string normalTxt, string encryptKey)
        {
            var bytes = Encoding.UTF8.GetBytes(normalTxt);
            var key = Encoding.UTF8.GetBytes(encryptKey);
            var iv = Encoding.UTF8.GetBytes(encryptKey);
            using (MemoryStream ms = new MemoryStream())
            {
                var encry = new DESCryptoServiceProvider();
                encry.Mode = CipherMode.ECB;
                CryptoStream cs = new CryptoStream(ms, encry.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string DesDecryptWithJava(string securityTxt, string encryptKey)
        {
            try
            {
                var bytes = Convert.FromBase64String(securityTxt);
                var key = Encoding.UTF8.GetBytes(encryptKey);
                var iv = Encoding.UTF8.GetBytes(encryptKey);
                using (MemoryStream ms = new MemoryStream())
                {
                    var descrypt = new DESCryptoServiceProvider();
                    descrypt.Mode = CipherMode.ECB;
                    CryptoStream cs = new CryptoStream(ms, descrypt.CreateDecryptor(key, iv), CryptoStreamMode.Write);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }

    public static class SecurityHelperExt
    {
        public static string ToMd5(this string data) => SecurityHelper.ToMD5(data);
    }
}
