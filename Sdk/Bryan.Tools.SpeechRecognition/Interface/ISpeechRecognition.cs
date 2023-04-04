using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tools.SpeechRecognition
{
    public interface ISpeechRecognition
    {
        /// <summary>
        /// 语音url转文字
        /// </summary>
        /// <param name="url">//语音 URL，公网可下载。当 SourceType 值为 0 时须填写该字段，为 1 时不填；URL 的长度大于 0，小于 2048，需进行urlencode编码。音频时间长度要小于60s。</param>
        /// <param name="engServiceType">引擎类型。8k：电话 8k 通用模型；16k：16k 通用模型。只支持单声道音频识别。</param>
        /// <param name="voiceFormat">识别音频的音频格式（支持mp3,wav）。</param>
        /// <param name="projectId">腾讯云项目 ID，可填 0，总长度不超过 1024 字节。</param>
        /// <param name="subServiceType">子服务类型。2，一句话识别。</param>
        /// <param name="sourceType">音数据来源。0：语音 URL；1：语音数据（post body）。</param>
        /// <returns></returns>
        public Task<SpeechRecognitionResponse> VideoToTextByTencent(string url, string engServiceType = "16k", ulong projectId = 0, ulong subServiceType = 2, ulong sourceType = 0);
    }
}
