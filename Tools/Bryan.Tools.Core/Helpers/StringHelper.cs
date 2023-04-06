using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class StringHelper
    {
        /// <summary>
        /// 计算相似度
        /// </summary>
        public static double CalculateSimilarity(string txt1, string txt2)
        {
            List<char> sl1 = txt1.ToCharArray().ToList();
            List<char> sl2 = txt2.ToCharArray().ToList();
            //去重
            List<char> sl = sl1.Union(sl2).ToList<char>();

            //获取重复次数
            List<int> arrA = new List<int>();
            List<int> arrB = new List<int>();
            foreach (var str in sl)
            {
                arrA.Add(sl1.Where(x => x == str).Count());
                arrB.Add(sl2.Where(x => x == str).Count());
            }
            //计算商
            double num = 0;
            //被除数
            double numA = 0;
            double numB = 0;
            for (int i = 0; i < sl.Count; i++)
            {
                num += arrA[i] * arrB[i];
                numA += Math.Pow(arrA[i], 2);
                numB += Math.Pow(arrB[i], 2);
            }
            double cos = num / (Math.Sqrt(numA) * Math.Sqrt(numB));
            return cos;
        }

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
