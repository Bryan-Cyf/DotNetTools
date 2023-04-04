using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class ImageHelper
    {
        /// <summary>
        /// 获取图片的字节流
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytes(string file)
        {
            using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes,0,(int) fileStream.Length);
                return bytes;
            }
        }

        /// <summary>
        /// 获取图片的base64编码数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> GetBase64String(string file)
        {
            return Convert.ToBase64String(await GetBytes(file));
        }

        /// <summary>
        /// base64 encoded image
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetBase64StringFromUrl(HttpClient httpClient, string url)
        {
            return Convert.ToBase64String(await httpClient.GetByteArrayAsync(url));
        }
    }
}
