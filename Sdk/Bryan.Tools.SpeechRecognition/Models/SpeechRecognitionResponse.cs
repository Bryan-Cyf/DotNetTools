namespace Tools.SpeechRecognition
{
    /// <summary>
    /// 一句话语音识别结果
    /// </summary>
    public class SpeechRecognitionResponse
    {
        public SpeechRecognitionResponse()
        {
            Response = new SpeechRecognitionDetail();
        }

        public SpeechRecognitionDetail Response { get; set; }
    }

    public class SpeechRecognitionDetail
    {
        /// <summary>
        /// 请求id
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 错误结果
        /// </summary>
        public SpeechRecognitionError Error { get; set; }
    }

    public class SpeechRecognitionError
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
