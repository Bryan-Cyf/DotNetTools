using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools
{
    public static class SysHelper
    {
        public static void GetIp(out string ip, out string addr)
        {
            ip = string.Empty;
            addr = string.Empty;
            HttpWebRequest request = null;
            try
            {
                request = HttpWebRequest.CreateHttp("http://2019.ip138.com/ic.asp");
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,ja;q=0.7");
                request.Headers.Add("Referer", "http://www.ip138.com/");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
                using (var response = request.GetResponse())
                {
                    using (var sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                    {
                        var html = sr.ReadToEnd();
                        var match = Regex.Match(html, @"<center>您的IP是：\[(.*?)\] 来自：(.*?)</center>");
                        if (match.Success)
                        {
                            ip = match.Groups[1].Value;
                            addr = match.Groups[2].Value;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                request?.Abort();
            }
        }
        public static string AppGuid
        {
            get
            {
                try
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    object[] attr = (asm.GetCustomAttributes(typeof(GuidAttribute), true));
                    return new Guid((attr[0] as GuidAttribute).Value).ToString("B").ToUpper();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        #region MAC
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
        IntPtr HDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        uint nInBufferSize,
        IntPtr lpOutBuffer,
        uint nOutBufferSize,
        ref uint lpBytesReturned,
        IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        private static string GetMac(string niId)
        {
            if (string.IsNullOrWhiteSpace(niId)) return null;

            System.IntPtr hDevice = CreateFile("\\\\.\\" + niId,
                                                0x80000000 | 0x40000000,
                                                0,
                                                IntPtr.Zero,
                                                3,
                                                4,
                                                IntPtr.Zero
                                                );
            if (hDevice.ToInt32() == -1)
            {
                return string.Empty;
            }
            uint Len = 0;
            IntPtr Buffer = Marshal.AllocHGlobal(256);

            Marshal.WriteInt32(Buffer, 0x01010101);
            if (!DeviceIoControl(hDevice,
                              0x170002,
                              Buffer,
                              4,
                              Buffer,
                              256,
                              ref Len,
                              IntPtr.Zero))
            {
                Marshal.FreeHGlobal(Buffer);
                CloseHandle(hDevice);
                return string.Empty;
            }
            byte[] macBytes = new byte[6];
            Marshal.Copy(Buffer, macBytes, 0, 6);
            Marshal.FreeHGlobal(Buffer);
            CloseHandle(hDevice);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < macBytes.Length; i++)
            {
                sb.Append(macBytes[i].ToString("X2"));
                if (i != macBytes.Length - 1)
                {
                    sb.Append(":");
                }

            }
            return sb.ToString();
        }

        private static NetworkInterface GetNetworkInterface()
        {
            IEnumerable<NetworkInterface> allNis = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => !string.IsNullOrEmpty(x.Id));
            //本地
            var nis = allNis.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet && !Regex.IsMatch(x.Description, "Hyper-V|"));
            //无线
            if (nis.Count() == 0)
            {
                nis = allNis.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            }

            return nis.FirstOrDefault() ?? throw new Exception("获取不到网卡");
        }

        /// <summary>
        /// 获取真正的网卡MAC地址
        /// </summary>
        public static string GetMac()
        {
            var ni = GetNetworkInterface();
            return GetMac(ni.Id);
        }

        public static string GetMac_test()
        {
            var ni = GetNetworkInterface();
            return ni.GetPhysicalAddress().ToString();
        }
        #endregion
    }
}
