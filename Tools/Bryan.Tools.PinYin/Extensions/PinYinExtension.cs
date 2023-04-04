using Tools.PinYin;

public static class PinYinExtension
{
    /// <summary>
    /// 汉字转拼音
    /// </summary>
    public static string ToPinyin(this string str)
    {
        return PinYinHelper.GetPinyin(str);
    }

    /// <summary>
    /// 汉字转拼音首字母
    /// </summary>
    public static string ToPinyinInitialLetter(this string str)
    {
        return PinYinHelper.GetPinyinInitialLetter(str);
    }
}
