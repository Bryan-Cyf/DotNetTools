using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class IpHelper
    {
        /// <summary>
        /// 获取互联网ip
        /// </summary>
        /// <returns></returns>
        public static async Task<(string Ip, string Addr)> GetRemoteIp()
        {
            using var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate });
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
            var jj = await (await client.GetAsync("https://ip.cn/api/index?ip=&type=0").ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
            var j = JsonConvert.DeserializeObject<JObject>(jj);
            var ip = j.Value<string>("ip");
            return (ip, null);
        }

        /// <summary>
        /// 获取本地ip
        /// </summary>
        /// <returns></returns>
        public static (string Ip, string Addr) GetLocalIp()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces(); //适配器
            var ip = adapters
                .Where(x => x.Name == "eth0" || x.Name.Contains("以太网"))
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                .Select(x => x.Address.ToString())
                .FirstOrDefault();
            return (ip, null);
        }
    }
}
