using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class StringHelper
    {
        /// <summary>
        /// 字符串批量替换
        /// </summary>
        /// <param name="s"></param>
        /// <param name="array"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string BatchReplace(string s, string[] array, string p)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            for (int i = 0; i < array.Length; i++)
            {
                s = s.Replace(array[i], p);
            }
            return s;
        }

        /// <summary>
        /// 清洗html,保留一个空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlReplace(string s)
        {
            return BatchReplace(s, new string[] { "\r", "\n", "\t", "&nbsp;", "  " }, "");
        }
    }
}
