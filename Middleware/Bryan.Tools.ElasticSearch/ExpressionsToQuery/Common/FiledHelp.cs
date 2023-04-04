using System.Linq;

namespace Tools.Elastic
{
    public class FiledHelp
    {
        public static string GetValues(object obj, string filed)
        {
            var type = obj.GetType().GetProperties().Where(x => x.Name == filed).Select(x => x.PropertyType.Name).FirstOrDefault();
            if (type == null) return filed;
            filed = filed.ToFirstLower();
            return type switch
            {
                "String" => filed + ".keyword",
                _ => filed
            };
        }

        public static string GetValues(string propertyTypeName, string filed)
        {
            filed = filed.ToFirstLower();
            return propertyTypeName switch
            {
                "String" => filed + ".keyword",
                _ => filed
            };
        }
    }
}