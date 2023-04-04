using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Tools
{
    public class WebHelper
    {
        public static string Ip => s_ip.Value;
        private static Lazy<string> s_ip = new Lazy<string>(() => GetIp().Result);
        public static string HostName=> s_hostName.Value;
        private static Lazy<string> s_hostName = new Lazy<string>(() => Dns.GetHostName());

        public static async Task<string> GetIp()
        {
            var ips = await Dns.GetHostAddressesAsync(HostName);
            return ips?.Where(it => it.AddressFamily == AddressFamily.InterNetwork)?.FirstOrDefault()?.ToString();
        }

        public static async Task<string> GetIpFromIp138()
        {
            while (true)
            {
                try
                {
                    using (var httpClient = new HttpClient(new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                        //Proxy = new WebProxy("127.0.0.1:8888")
                    }))
                    {
                        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                        httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,ja;q=0.4");
                        httpClient.DefaultRequestHeaders.Add("Referer", "http://www.ip138.com/");
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0");

                        //var httpResponse =await httpClient.GetAsync("http://2017.ip138.com/ic.asp");
                        //var result = await httpResponse.Content.ReadAsStringAsync();
                        var result = await httpClient.GetStringAsync("http://2017.ip138.com/ic.asp");
                        return Regex.Match(result, @"\[([\d\.]+)\]").Groups[1].Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await Task.Delay(1000);
                }
            }
        }

        public static async Task<string> GetIpFromBaidu()
        {
            while (true)
            {
                try
                {
                    using (var httpClient = new HttpClient(new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                        //Proxy = new WebProxy("127.0.0.1:8888")
                    }))
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(10);
                        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                        httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,ja;q=0.4");
                        //httpClient.DefaultRequestHeaders.Add("Referer", "http://www.ip138.com/");
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0");

                        //var httpResponse =await httpClient.GetAsync("http://2017.ip138.com/ic.asp");
                        //var result = await httpResponse.Content.ReadAsStringAsync();
                        var result = await httpClient.GetStringAsync("https://www.baidu.com/s?wd=ip&ie=UTF-8");
                        return Regex.Match(result, @"fk=""([\d\.]+)""").Groups[1].Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await Task.Delay(1000);
                }
            }
        }

        public static async Task<List<string>> GetIps()
        {
            var ips = await Dns.GetHostAddressesAsync(HostName);
            return ips?.Select(it => it.ToString()).ToList();
        }
    }
}
