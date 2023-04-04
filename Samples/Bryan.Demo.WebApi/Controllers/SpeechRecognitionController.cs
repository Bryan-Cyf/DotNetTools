using Microsoft.AspNetCore.Mvc;
using Tools.SpeechRecognition;

namespace Helper.WebApi.Test.Controllers
{
    public class SpeechRecognitionController : BaseController
    {
        private readonly ISpeechRecognition _asrApi;
        public SpeechRecognitionController(ISpeechRecognition asrApi)
        {
            _asrApi = asrApi;
        }

        [HttpGet]
        public async Task<SpeechRecognitionResponse> VideoToText([FromQuery] string url)
        {
            //url = "https://mbridge-uat.oss-cn-shenzhen.aliyuncs.com/ap/m/in/20221114/101073/chat/16/166841157165526449.mp3";
            var result = await _asrApi.VideoToTextByTencent(url);
            if (result.IsSuccess())
            {

            }
            return result;
        }
    }
}
