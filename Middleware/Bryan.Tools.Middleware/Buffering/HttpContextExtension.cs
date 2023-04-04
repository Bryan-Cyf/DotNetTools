using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Tools.Middleware.Buffering
{
    /// <summary>
    /// HttpContext扩展类
    /// </summary>
    internal static class HttpContextExtension
    {
        /// <summary>
        /// 获取请求的Get/Post参数
        /// 需要先用UseBuffering开启多次读取才能读Post body参数
        /// </summary>
        internal static string GetHttpParameters(this HttpContext context)
        {
            string data = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(context.Request.ContentType) && context.Request.ContentType.Contains("multipart/form-data"))
                {
                    var files = context.Request.Form.Files;

                    if (files.Count > 0)
                    {
                        var fileName = string.Join(",", files.Select(a => a.FileName));
                        return data = "上传文件：" + fileName;
                    }
                }
                NameValueCollection form = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
                HttpRequest request = context.Request;

                switch (request.Method)
                {
                    case "POST":
                        using (var mem = new MemoryStream())
                        using (var reader = new StreamReader(mem))
                        {
                            request.Body.Seek(0, SeekOrigin.Begin);
                            request.Body.CopyTo(mem);
                            mem.Seek(0, SeekOrigin.Begin);
                            data = reader.ReadToEnd();
                            request.Body.Position = 0;
                        }
                        break;
                    case "GET":
                        IDictionary<string, string> parameters = new Dictionary<string, string>();
                        for (int f = 0; f < form.Count; f++)
                        {
                            string key = form.Keys[f];
                            parameters.Add(key, form[key]);
                        }
                        IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
                        IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();
                        StringBuilder query = new StringBuilder();
                        while (dem.MoveNext())
                        {
                            string key = dem.Current.Key;
                            string value = dem.Current.Value;
                            if (!string.IsNullOrEmpty(key))
                            {
                                query.Append(key).Append("=").Append(value).Append("&");
                            }
                        }
                        data = query.ToString().TrimEnd('&');
                        break;
                    default:
                        data = string.Empty;

                        break;
                }
                return data;
            }
            catch
            {
                return data;
            }
        }
    }
}
