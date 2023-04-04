using System.Collections.Generic;

namespace Tools.SpeechRecognition
{
    /// <summary>
    /// 配置
    /// </summary>
    public class SpeechRecognitionOptions
    {

        public const string SectionName = "SpeechRecognition";

        /// <summary>
        /// 秘钥Id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// 秘钥Key
        /// </summary>
        public string SecretKey { get; set; }
    }
}