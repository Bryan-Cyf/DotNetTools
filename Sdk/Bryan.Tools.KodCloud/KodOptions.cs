using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KodSdk
{
    public class KodOptions
    {
        private string _loginToken;

        public LoginModeEnum LoginMode { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 用户名(*)
        /// </summary>
        public string Name { get; set; }
        public string ApiLoginToken { get; set; }
        public string LoginToken
        {
            get
            {
                if (string.IsNullOrEmpty(_loginToken))
                {
                    if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(ApiLoginToken))
                    {
                        _loginToken = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Name))}|{KodClient.Md5($"{Name}{ApiLoginToken}")}";
                    }
                    else throw new ArgumentNullException(nameof(LoginToken));
                }
                return _loginToken;
            }
            set => _loginToken = value;
        }
        public string BaseAddress { get; set; }
        public enum LoginModeEnum
        {
            用户名密码 = 1,
            服务端免密登录 = 2,
        }
    }
}
