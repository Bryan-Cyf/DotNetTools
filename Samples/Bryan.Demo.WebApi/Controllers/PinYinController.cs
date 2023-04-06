using Microsoft.AspNetCore.Mvc;
using Tools.PinYin;
using Tools.SnowFlakeId;

namespace Bryan.Demo.WebApi.Controllers
{
    public class PinYinController : BaseController
    {

        /// <summary>
        /// 通过扩展方法实现
        /// </summary>
        [HttpGet]
        public string GetPinYinByExtension([FromQuery] string str)
        {
            var result = str.ToPinyin();
            return result;
        }

        /// <summary>
        /// 通过帮助类实现
        /// </summary>
        [HttpGet]
        public string GetPinYinByHelper([FromQuery] string str)
        {
            var result = PinYinHelper.GetPinyin(str);
            return result;
        }

        /// <summary>
        /// 通过扩展方法实现
        /// </summary>
        [HttpGet]
        public string GetPinyinInitialLetterByExtension([FromQuery] string str)
        {
            var result = str.ToPinyinInitialLetter();
            return result;
        }

        /// <summary>
        /// 通过帮助类实现
        /// </summary>
        [HttpGet]
        public string GetPinyinInitialLetterByHelper([FromQuery] string str)
        {
            var result = PinYinHelper.GetPinyinInitialLetter(str);
            return result;
        }
    }
}
