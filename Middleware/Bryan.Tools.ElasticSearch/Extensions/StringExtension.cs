namespace Tools.Elastic
{
    public static class StringExtension
    {
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFirstLower(this string str) 
        {
            return str[..1].ToLower() + str[1..];
        }
    }
}