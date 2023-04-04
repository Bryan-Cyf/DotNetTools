using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tools
{
    public class CookieHelper
    {
        public static CookieContainer StringToCC(string cookie, string domain = ".")
        {
            {
                CookieContainer cc = new CookieContainer();
                cookie = cookie.Replace(",", "%2C");//C# cookie不识别“,”
                string[] tempCookies = cookie.Split(';');
                string tempCookie = null;
                int Equallength = 0;//  等号的位置 
                string cookieKey = null;
                string cookieValue = null;
                for (int i = 0; i < tempCookies.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tempCookies[i]))
                    {
                        tempCookie = tempCookies[i];
                        Equallength = tempCookie.IndexOf("=");
                        if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=; 
                        {
                            cookieKey = tempCookie.Substring(0, Equallength).Trim();
                            //cookie= 
                            if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=; 
                            {
                                cookieValue = "";
                            }
                            else
                            {
                                cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                            }
                        }
                        else
                        {
                            cookieKey = tempCookie.Trim();
                            cookieValue = "";
                        }
                        var c = new System.Net.Cookie(cookieKey, cookieValue, "/", domain);
                        //Console.WriteLine(cookieKey + "-------------" + cookieValue);
                        //Console.WriteLine();
                        cc.Add(c);
                    }
                }
                return cc;
            }
        }

        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }

        public static string CookieToString(CookieContainer cc)
        {
            StringBuilder sbc = new StringBuilder();
            List<Cookie> cooklist = GetAllCookies(cc);
            foreach (Cookie cookie in cooklist)
            {
                sbc.AppendFormat("【{0}】-【{1}】-【{2}】-【{3}】-【{4}】-【{5}】\r\n",
                    cookie.Domain, cookie.Name, cookie.Path, cookie.Port,
                    cookie.Secure.ToString(), cookie.Value);
            }
            return sbc.ToString();
        }
    }
}
