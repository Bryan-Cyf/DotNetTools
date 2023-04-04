using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Tools
{
    public static class HttpClientHelper
    {
        public static HttpClient CreateHttpClient(Config config)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = config.AllowAutoRedirect,
                AutomaticDecompression = config.AutomaticDecompression,
                UseCookies = config.UseCookies,
                CookieContainer = config.CookieContainer,
                Proxy = config.ProxyEndpoint == null ? null :
                new WebProxy(config.ProxyEndpoint)
                {
                    Credentials = new NetworkCredential(config.ProxyUserName, config.ProxyPassword)
                },
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = config.Timeout;
            httpClient.DefaultRequestHeaders.ConnectionClose = config.ConnectionClose;
            for (int i = 0; i < config.DefaultRequestHeaders?.Count; i++)
            {
                var header = config.DefaultRequestHeaders.ElementAt(i);
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return httpClient;
        }

        public class Config
        {
            /// <summary>
            /// 默认不使用代理
            /// </summary>
            public string ProxyEndpoint { get; set; } = null;
            public string ProxyUserName { get; set; } = null;
            public string ProxyPassword { get; set; } = null;
            /// <summary>
            /// 默认不重定向
            /// </summary>
            public bool AllowAutoRedirect { get; set; } = false;
            public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            /// <summary>
            /// 默认不使用cookie
            /// </summary>
            public bool UseCookies { get; set; } = false;
            public CookieContainer CookieContainer { get; set; } = null;
            /// <summary>
            /// 默认超时时间
            /// </summary>
            public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
            public Dictionary<string, string> DefaultRequestHeaders { get; set; } = new Dictionary<string, string> {
                { "Accept", "text/html, application/xhtml+xml, image/jxr, */*" },
                { "Accept-Encoding","gzip, deflate"},
                //{ "Connection","close"},
            };
            public bool ConnectionClose { get; set; } = true;
        }

        private static HashSet<string> _httpDomain = new HashSet<string>();
        private static HttpClientHandler _httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        };

        private static readonly HttpClient _client = new HttpClient(_httpClientHandler)
        {
            //调试超时时间设定长一些，避免超时
            Timeout = TimeSpan.FromMilliseconds(60000)
        };

        /// <summary>
        /// 发起POST异步请求
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="timeOut"></param>
        /// <param name="headers">填充消息头</param>        
        /// <returns></returns>
        public static async Task<T> HttpPostAsync<T>(string url, string postData = null,
            string contentType = "application/json", Dictionary<string, string> headers = null) where T : class
        {
            var uri = new Uri(url);
            T result;
            var resultContent = "";
            var response = new HttpResponseMessage();

            setDNSTTL(uri);
            using (var message = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                message.Content = new StringContent(postData, Encoding.UTF8);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                response = await _client.SendAsync(message);
            }

            resultContent = await response.Content.ReadAsStringAsync();
            result = resultContent?.ToObj<T>();

            return result;
        }

        /// <summary>
        /// 发起GET异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url, string contentType = null,
            Dictionary<string, string> dictParams = null, Dictionary<string, string> headers = null) where T : class
        {
            if (dictParams != null)
            {
                foreach (var item in dictParams)
                {
                    if (url.Contains("?"))
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }

                    url += $"{item.Key}={item.Value}";
                }
            }

            var uri = new Uri(url);
            StringBuilder info = new StringBuilder();
            HttpResponseMessage response = null;
            string resultContent = string.Empty;
            setDNSTTL(uri);
            using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                message.Content = new StringContent(string.Empty, Encoding.UTF8);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }

                    info.Append($"header:{headers.ToJson()}");
                }

                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    info.Append($"contentType:{contentType}");
                }

                response = await _client.SendAsync(message);
            }

            resultContent = await response.Content.ReadAsStringAsync();

            return resultContent.ToObj<T>();
        }

        /// <summary>
        /// 发起POST异步请求，响应Strem Bytes
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<byte[]> PostBytesAsync(string url, string postData, Dictionary<string, string> headers = null)
        {
            byte[] bytes;
            var uri = new Uri(url);

            setDNSTTL(uri);
            using (var message = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                message.Content = new StringContent(postData, Encoding.UTF8, "application/json");
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }
                HttpResponseMessage response = await _client.SendAsync(message);

                var stream = await response.Content.ReadAsStreamAsync();
                bytes = new byte[stream.Length];

                stream.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }

        public static async Task<byte[]> GetBytesAsync(string url)
        {
            byte[] bytes;
            var uri = new Uri(url);
            HttpResponseMessage response = null;
            try
            {
                setDNSTTL(uri);
                using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    response = await _client.SendAsync(message);
                    var stream = await response.Content.ReadAsStreamAsync();
                    bytes = new byte[stream.Length];

                    stream.Read(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bytes;
        }
        /// <summary>
        /// 重写设置tcp的DNS生存期为1分钟（默认为1小时），定时回收HttpClient的连接。（背景：DNS改变后，该连接将指向无效DNS）
        /// </summary>
        /// <param name="uri"></param>
        private static void setDNSTTL(Uri uri)
        {
            if (!_httpDomain.Contains(uri.Host))
            {
                //写入新域名记录
                _httpDomain.Add(uri.Host);
                //设置新域名的TCP过期时间
                var sp = ServicePointManager.FindServicePoint(uri);
                if (sp != null)
                {
                    sp.ConnectionLeaseTimeout = 60 * 1000; //1分钟
                }
            }
        }
    }
}
