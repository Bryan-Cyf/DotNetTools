using System;
using Tools;

public static class JsonExtensions
{
    public static string ToJson(this object obj)
    {
        return JsonHelper.SerializeObject(obj);
    }

    public static string ToJsonIndented(this object obj)
    {
        return JsonHelper.SerializeObjectIndented(obj);
    }

    public static T ToObj<T>(this string json) where T : class
    {
        return JsonHelper.Deserialize<T>(json);
    }

    public static T ToJsonObj<T>(this string json, ref T anonymousTypeObject) where T : class
    {
        return JsonHelper.DeserializeAnonymousType<T>(json, ref anonymousTypeObject);
    }
    public static bool TryParseJson<T>(this string json, out T obj, T TanonymousTypeObject)
    {
        obj = default;
        try
        {
            obj = JsonHelper.DeserializeAnonymousType<T>(json, ref TanonymousTypeObject);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

