using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.SpeechRecognition
{
    public static class SpeechRecognitionExtension
    {
        public static bool IsSuccess(this SpeechRecognitionResponse response)
        {
            return response?.Response?.Error == null;
        }
    }
}
