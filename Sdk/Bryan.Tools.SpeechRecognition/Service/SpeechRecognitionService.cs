using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tools;

namespace Tools.SpeechRecognition
{
    /// <summary>
    /// Automatic Speech Recognition
    /// 自动语音识别技术
    /// </summary>
    internal class SpeechRecognitionService : ISpeechRecognition
    {
        private readonly SpeechRecognitionOptions _options;

        public SpeechRecognitionService(IOptionsSnapshot<SpeechRecognitionOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// 腾讯云-语音url转文字
        /// </summary>
        /// <param name="url">//语音 URL，公网可下载。当 SourceType 值为 0 时须填写该字段，为 1 时不填；URL 的长度大于 0，小于 2048，需进行urlencode编码。音频时间长度要小于60s。</param>
        /// <param name="engServiceType">引擎类型。8k：电话 8k 通用模型；16k：16k 通用模型。只支持单声道音频识别。</param>
        /// <param name="voiceFormat">识别音频的音频格式（支持mp3,wav）。</param>
        /// <param name="projectId">腾讯云项目 ID，可填 0，总长度不超过 1024 字节。</param>
        /// <param name="subServiceType">子服务类型。2，一句话识别。</param>
        /// <param name="sourceType">音数据来源。0：语音 URL；1：语音数据（post body）。</param>
        /// <returns></returns>
        public async Task<SpeechRecognitionResponse> VideoToTextByTencent(string url, string engServiceType = "16k", ulong projectId = 0, ulong subServiceType = 2, ulong sourceType = 0)
        {
            var result = new SpeechRecognitionResponse();
            url = HttpUtility.UrlDecode(url);//解密
            var handlerExtension = new List<string>() { "wav", "mp3" };
            string voiceFormat;
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }
            else
            {
                //判断类型
                voiceFormat = Path.GetExtension(url);
                voiceFormat = voiceFormat.TrimStart('.').ToLower();
                //只能转mp3和wav
                if (!handlerExtension.Contains(voiceFormat))
                {
                    return result;
                }
            }

            string apiUrl = "asr.tencentcloudapi.com";

            //参数
            SortedDictionary<string, object> signtureParam = new SortedDictionary<string, object>();
            signtureParam.Add("Action", "SentenceRecognition");
            signtureParam.Add("EngSerViceType", engServiceType);
            signtureParam.Add("ProjectId", projectId);
            signtureParam.Add("Region", "ap-guangzhou");
            signtureParam.Add("SecretId", _options.SecretId);
            signtureParam.Add("SourceType", sourceType);
            signtureParam.Add("SubServiceType", subServiceType);
            int timeStamp = SpeechRecognitionHelper.ConvertDateToInt(DateTime.Now);
            signtureParam.Add("Timestamp", timeStamp);
            signtureParam.Add("Url", url);
            signtureParam.Add("UsrAudioKey", Guid.NewGuid().ToString());
            signtureParam.Add("Version", "2019-06-14");
            signtureParam.Add("VoiceFormat", voiceFormat);
            signtureParam.Add("Nonce", "9759");
            //把参数拼接成Url参数
            var urlParam = SpeechRecognitionHelper.ToUrl(signtureParam);

            //参数加地址
            string urlAll = "GET" + apiUrl + "/?" + urlParam;
            //sha1签名
            string sha1 = SpeechRecognitionHelper.GetHmacSha1(urlAll, _options.SecretKey);
            //签名后url编码
            string sha1Encode = System.Web.HttpUtility.UrlEncode(sha1, Encoding.UTF8);
            //参数拼上签名
            signtureParam.Add("Signature", sha1Encode);
            var requestParam = SpeechRecognitionHelper.ToUrl(signtureParam);
            //得到最终请求
            var requestUrl = "https://" + apiUrl + "/?" + requestParam;
            //发送请求
            var response = await HttpClientHelper.HttpGetAsync<SpeechRecognitionResponse>(requestUrl);

            result = response;

            return result;

        }

    }
}
