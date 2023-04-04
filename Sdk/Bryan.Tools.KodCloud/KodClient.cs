using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KodSdk
{
    public class KodClient
    {
        private readonly KodOptions _options;
        private HttpClient _httpClient;
        private string _accessToken;
        private CookieContainer _cookieContainer;
        private string _cookieStr => _cookieContainer.GetCookieHeader(new Uri(_options.BaseAddress));

        public KodClient(KodOptions options)
        {
            _options = options;
            _cookieContainer = new CookieContainer();
            _httpClient = new HttpClient(new HttpClientHandler
            {
#if DEBUG
                Proxy = new WebProxy("127.0.0.1:8888"),
#endif
                CookieContainer = _cookieContainer,
            })
            {
                BaseAddress = new Uri(_options.BaseAddress),
            };
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        public async Task<bool> Login()
        {
            if (!string.IsNullOrEmpty(_accessToken)) return true;

            string content = null;
            switch (_options.LoginMode)
            {
                case KodOptions.LoginModeEnum.用户名密码:
                    if (string.IsNullOrEmpty(_options.Name) || string.IsNullOrEmpty(_options.Password)) throw new ArgumentNullException();
                    content = await _httpClient.GetStringAsync($"/?user/index/loginSubmit&name={_options.Name}&password={_options.Password}").ConfigureAwait(false);
                    break;
                case KodOptions.LoginModeEnum.服务端免密登录:
                    if (string.IsNullOrEmpty(_options.LoginToken)) throw new ArgumentNullException(nameof(_options.LoginToken));
                    content = await _httpClient.GetStringAsync($"/?user/index/loginSubmit&loginToken={_options.LoginToken}").ConfigureAwait(false);
                    break;
                default:
                    throw new Exception("请配置LoginMode");
            }
            var j = JObject.Parse(content);
            _accessToken = j.Value<string>("info");
            //var csrfToken = Regex.Match(_cookieStr, "CSRF_TOKEN=(.*?);").Groups[1].Value;
            //_httpClient.DefaultRequestHeaders.Add("CSRF_TOKEN", csrfToken);
            return j.Value<bool>("code");
        }

        [Obsolete("UploadByChunk")]
        public async Task<bool> Upload(string filePath, string path, string name)
        {
            await Login();

            var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/upload/fileUpload&accessToken={_accessToken}");
            string boundary = string.Format("----Boundary{0}", DateTime.Now.Ticks.ToString("x"));
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            using FileStream fStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            content.Add(new StreamContent(fStream, (int)fStream.Length), "file", filePath);
            content.Add(new StringContent(path), "path");
            content.Add(new StringContent(name), "name");
            hrm.Content = content;
            var resp = await _httpClient.SendAsync(hrm).ConfigureAwait(false);
            var ret = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var j = JObject.Parse(ret);
            return j.Value<bool>("code");
        }

        /// <summary>
        /// 文件分片上传
        /// https://doc.kodcloud.com/v2/#/explorer/file?id=_15-%e6%96%87%e4%bb%b6%e4%b8%8a%e4%bc%a0
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<string> UploadByChunk(string filePath, string path, string name)
        {
            var fileData = File.ReadAllBytes(filePath);
            return UploadByChunk(fileData, path, name);
        }

        public async Task<string> UploadByChunk(byte[] fileData, string path, string name)
        {
            await Login();

            var chunkSize = 1 << 19;
            var chunks = (int)Math.Ceiling((fileData.Length * 1.0) / chunkSize);
            string boundary = string.Format("----Boundary{0}", DateTime.Now.Ticks.ToString("x"));
            var queue = new ConcurrentQueue<int>(Enumerable.Range(0, chunks));
            var resultQueue = new ConcurrentQueue<JObject>();
            var tasks = Enumerable.Range(0, Environment.ProcessorCount * 2).Select(x => Task.Run(async () =>
            {
                while (queue.TryDequeue(out var i))
                {
                    JObject jo = null;
                    for (int j = 0; j < 3; j++)
                    {
                        var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/upload/fileUpload&accessToken={_accessToken}");
                        MultipartFormDataContent content = new MultipartFormDataContent(boundary);
                        content.Headers.Remove("Content-Type");
                        content.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
                        var chunkData = fileData.Skip(i * chunkSize).Take(chunkSize).ToArray();
                        using var s = new MemoryStream(chunkData);
                        content.Add(new StreamContent(s, (int)s.Length), "file", name);
                        content.Add(new StringContent(path), "path");
                        content.Add(new StringContent(name), "name");
                        content.Add(new StringContent(fileData.Length.ToString()), "size");
                        content.Add(new StringContent(chunkSize.ToString()), "chunkSize");
                        content.Add(new StringContent(chunks.ToString()), "chunks");
                        content.Add(new StringContent(i.ToString()), "chunk");
                        hrm.Content = content;
                        var resp = await _httpClient.SendAsync(hrm).ConfigureAwait(false);
                        var ret = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                        Log(ret);
                        jo = JObject.Parse(ret);
                        //保存成功的结果
                        if (jo.Value<bool>("code"))
                        {
                            resultQueue.Enqueue(jo);
                            break;
                        }
                    }
                }
            }));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            if (resultQueue.ToList().All(x => x.Value<string>("info") == null))
            {
                throw new Exception("上传文件失败");
            }
            return await CompareMd5(fileData, path, name);
        }

        /// <summary>
        /// 获取文件属性
        /// https://doc.kodcloud.com/v2/#/explorer/file?id=_2-%e8%8e%b7%e5%8f%96%e6%96%87%e4%bb%b6%e5%b1%9e%e6%80%a7
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public async Task<JObject> FileInfo(string path, string name)
        {
            await Login();

            var body = new
            {
                dataArr = new List<object>{
                    new {
                        path = $"{path.TrimEnd('/')}/{name}",
                        //name = name,
                        //type = "file",
                    }
                }
            };
            var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/index/pathInfo&accessToken={_accessToken}");
            hrm.Content = new StringContent("dataArr=" + JsonConvert.SerializeObject(body.dataArr), Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _httpClient.SendAsync(hrm).ConfigureAwait(false);
            var ret = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jo = JObject.Parse(ret);
            return jo;

            //var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/editor/fileGet&accessToken={_accessToken}");
            //hrm.Content = new StringContent("path=" + $"{path.TrimEnd('/')}/{name}", Encoding.UTF8, "application/x-www-form-urlencoded");
            //var resp = await _httpClient.SendAsync(hrm).ConfigureAwait(false);
            //var ret = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            //var jo = JObject.Parse(ret);
        }

        internal static string Md5(string msg)
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(msg));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        internal static string Md5(byte[] data)
        {
            try
            {
                using var ms = new MemoryStream(data);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(ms);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
            }
        }

        internal async Task<string> CompareMd5(byte[] data, string path, string name)
        {
            var fileMd5 = Md5(data);
            var fileInfo = await FileInfo(path, name).ConfigureAwait(false);
            var remoteFileMd5 = fileInfo.SelectToken("$.data")?.Value<string>("hashMd5");
            if (!fileMd5.Equals(remoteFileMd5)) throw new Exception("md5校验失败");
            return fileMd5;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Sort()
        {
            await Login();
            var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?user/setting/setConfig?&accessToken={_accessToken}");
            hrm.Content = new StringContent($"key=listSortField&value=modifyTime", Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _httpClient.SendAsync(hrm).ContinueWith(x => x.Result.Content.ReadAsStringAsync()).Unwrap();

            hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?user/setting/setConfig?&accessToken={_accessToken}");
            hrm.Content = new StringContent($"key=listSortOrder&value=down", Encoding.UTF8, "application/x-www-form-urlencoded");
            resp = await _httpClient.SendAsync(hrm).ContinueWith(x => x.Result.Content.ReadAsStringAsync()).Unwrap();
            return resp.Contains("修改已生效");
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> DelFile(string path)
        {
            await Login();
            var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/index/pathDelete&accessToken={_accessToken}");
            hrm.Content = new StringContent("dataArr=[{\"type\":\"file\", \"path\":\"" + path + "\"}]", Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _httpClient.SendAsync(hrm).ContinueWith(x => x.Result.Content.ReadAsStringAsync()).Unwrap();
            Log(resp);
            return resp.Contains("删除成功");
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> GetDirInfo(string path)
        {
            await Login();

            var hrm = new HttpRequestMessage(HttpMethod.Post, $"/index.php?explorer/list/path&accessToken={_accessToken}");
            hrm.Content = new StringContent($"path={path}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _httpClient.SendAsync(hrm).ContinueWith(x => x.Result.Content.ReadAsStringAsync()).Unwrap();
            Log(resp);
            return resp;
        }

        private void Log(string msg)
        {
            Console.WriteLine("==============================="
                + Environment.NewLine + msg.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", ""));
        }
    }
}
